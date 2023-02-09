using System.Collections.Immutable;
using System.Xml;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypealizR.Core;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;
internal sealed class EmptyFile : AdditionalText
{
    private readonly string text = "";

    public override string Path { get; }

    public EmptyFile(string path)
    {
        Path = path;
    }

    public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken())
    {
        return SourceText.From(text);
    }
}

public class RessourceFile_Tests
{
    private sealed record LineInfo(int LineNumber = 42, int LinePosition = 1337, bool HasLineInfo = true) : IXmlLineInfo
    {
        bool IXmlLineInfo.HasLineInfo() => HasLineInfo;
    }

    [Theory]
    [InlineData(3,
        "Ressource1.resx",
        "Ressource2.resx",
        "Ressource3.resx"
    )]
    [InlineData(3,
        "A/Ressource1.resx",
        "A/Ressource2.resx",
        "B/Ressource1.resx"
    )]
    public void Parsing_Paths_Does_Not_Group_Files_With_Different_Names_And_Paths(int expected, params string[] paths)
    {
        var additionalFiles = paths
            .Select(x => new AdditionalTextWithOptions(new EmptyFile(x), GeneratorTesterOptions.Empty))
            .ToArray();

        var actual = RessourceFile.From(ImmutableArray.Create(additionalFiles), CancellationToken.None);
        actual.Should().HaveCount(expected);
    }

    [Theory]
    [InlineData(2,
        "Ressource1.resx",          //<--
        "Ressource1.de.resx",
        "Ressource1.de-DE.resx",
        "Ressource1.en-US.resx",
        "Ressource1.en-EN.resx",
        "Ressource2.resx",          //<--
        "Ressource2.de.resx",
        "Ressource2.de-DE.resx",
        "Ressource2.en-US.resx",
        "Ressource2.en-EN.resx",
        "Ressource3.en-EN.resx",    //<--ignored
        "Ressource4.en.resx"        //<--ignored
    )]
    [InlineData(2,
        "A/Ressource1.resx",          //<--
        "A/Ressource1.de.resx",
        "A/Ressource1.de-DE.resx",
        "A/Ressource1.en-US.resx",
        "A/Ressource1.en-EN.resx",
        "B/Ressource1.resx",          //<--
        "B/Ressource1.de.resx",
        "B/Ressource1.de-DE.resx",
        "B/Ressource1.en-US.resx",
        "B/Ressource1.en-EN.resx",
        "B/Ressource3.en-EN.resx",    //<--ignored
        "B/Ressource4.en.resx",       //<--ignored
        "C/Ressource1.en-EN.resx",    //<--ignored
        "C/Ressource2.en.resx"        //<--ignored
    )]
    public void Parsing_Paths_Groups_Localizations_By_Path(int expected, params string[] paths)
    {
        var additionalFiles = paths
            .Select(x => new AdditionalTextWithOptions(new EmptyFile(x), GeneratorTesterOptions.Empty))
            .ToArray();

        var actual = RessourceFile.From(ImmutableArray.Create(additionalFiles), CancellationToken.None);

        actual.Should().HaveCount(expected);
    }

    [Theory]
    [InlineData("SomeFile", @"SomeFile")]
    [InlineData("SomeFile", @"SomeFile.resx")]
    [InlineData("SomeFile", @"SomeFile.de.resx")]
    [InlineData("SomeFile", @"SomeFile.de-DE.resx")]
    [InlineData("SomeFile", @"SomeRelativePath/SomeFile")]
    [InlineData("SomeFile", @"SomeRelativePath/SomeFile.resx")]
    [InlineData("SomeFile", @"SomeRelativePath/SomeFile.de.resx")]
    [InlineData("SomeFile", @"SomeRelativePath/SomeFile.de-DE.resx")]
    [InlineData("SomeFile", @"c:/SomeRelativePath/SomeFile")]
    [InlineData("SomeFile", @"c:/SomeRelativePath/SomeFile.resx")]
    [InlineData("SomeFile", @"c:/SomeRelativePath/SomeFile.de.resx")]
    [InlineData("SomeFile", @"c:/SomeRelativePath/SomeFile.de-DE.resx")]
    public void SimpleFileNameOf_Reduces_All_Additional_Extensions(string expected, string input)
    {
        var actual = RessourceFile.GetSimpleFileNameOf(input);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", "", "Hello")]
    [InlineData("Hello [world]", "", "Hello [world]")]
    [InlineData("Hello [world]:", "", "Hello [world]:")]
    [InlineData("[]: world", "", "[]: world")]
    [InlineData("[world]:", "", "[world]:")]
    [InlineData("[world]: ", "world", "[world]: ")]
    [InlineData("[world]:  ", "world", "[world]:  ")]
    [InlineData("[Hello] world", "", "[Hello] world")]
    [InlineData("[Hello]world", "", "[Hello]world")]
    [InlineData("[]: Hello", "", "[]: Hello")]
    [InlineData("[logs]: Hello [world]", "logs", "Hello [world]")]
    [InlineData("[message.info]: Hello [world]:", "message.info", "Hello [world]:")]
    [InlineData("[Hello]: world", "Hello", "world")]
    [InlineData("[Hello]:world", "Hello", "world")]
    [InlineData(" [Hello]:world", "Hello", "world")]
    [InlineData("[ Hello]:world", "Hello", "world")]
    [InlineData("[Hello ]:world", "Hello", "world")]
    [InlineData("[ Hello ]:world", "Hello", "world")]
    [InlineData("[ Hello.]:world", "Hello", "world")]
    [InlineData("[ Hello. ]:world", "Hello", "world")]
    [InlineData("[ Hello .]:world", "Hello", "world")]
    [InlineData("[ Hello . ]:world", "Hello", "world")]
    [InlineData("[Hello .World]:world", "Hello.World", "world")]
    [InlineData("[Hello. World]:world", "Hello.World", "world")]
    [InlineData("[ Hello. World]:world", "Hello.World", "world")]
    [InlineData("[ Hello . World]:world", "Hello.World", "world")]
    [InlineData("[ Hello .World ]:world", "Hello.World", "world")]
    [InlineData("[ Hello. World ]:world", "Hello.World", "world")]
    [InlineData("[ Hello . World ]:world", "Hello.World", "world")]
    [InlineData("[ Hello .World.]:world", "Hello.World", "world")]
    [InlineData("[ Hello .World. ]:world", "Hello.World", "world")]
    [InlineData(@"[Hello:?!§$&%\/.World. ]:world", "Hello.World", "world")]
    [InlineData(@"[Hello?!§$&%\/:?!§$&%\/.World. ]:world", "Hello.World", "world")]
    [InlineData(@"[Hello?!§$&%\/:?!§$&%\/.World?!§$&%\/:?!§$&%\/]:world", "Hello.World", "world")]
    [InlineData(@"[Hello?_World]:world", "Hello_World", "world")]
    public void Entry_Extracts_Groups(string input, string expectedGroupKey, string expectedKey)
    {
        var sut = new RessourceFileEntry(input, input, new LineInfo());
        var actualGroupKey = string.Join('.', sut.Groups);
        actualGroupKey.Should().Be(expectedGroupKey);

        sut.Key.Should().Be(expectedKey);
    }
}
