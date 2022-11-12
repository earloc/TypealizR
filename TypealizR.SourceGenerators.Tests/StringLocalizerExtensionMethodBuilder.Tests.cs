using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TypealizR.SourceGenerators.StringLocalizer;
using TypealizR.SourceGenerators.Tests;

namespace TypealizR.SourceGenerators;

public class StringLocalizerExtensionMethodBuilder_Tests
{
    private static XmlLineInfo anywhere = new()
    {
        LineNumber = 0,
        LinePosition = 0
    };

    private static TypeInfo targetType = new ("Name.Space", "TypeName");

    [Theory]
    [InlineData("Name", "Name")]
    [InlineData("Hello World!", "Hello_World")]
    [InlineData("Hello World.", "Hello_World")]
    [InlineData("Hello World?", "Hello_World")]
    [InlineData("Hello {0}", "Hello__0")]
    [InlineData("{0} Hello {0}", "Hello__0")]
    [InlineData("Hello {name}", "Hello__name")]
    [InlineData("Hello {name}, today is {now}", "Hello__name__today_is__now")]
    public void Ensures_Compilable_ExtensionMethodName(string input, string expected)
    {
        var sut = new StringLocalizerExtensionMethodBuilder(input, input, anywhere);
        var method = sut.Build(targetType);

        var actual = method.Name;
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello {0}", "object _0")]
    [InlineData("Hello {name}", "object name")]
    [InlineData("Hello {0}, today is {1}", "object _0", "object _1")]
    [InlineData("Hello {name}, today is {date}", "object name", "object date")]
    [InlineData("{date} Hello {name}, today is {date}", "object name", "object date")]
    public void Extracts_Parameters(string input, params string[] expected)
    {
        var sut = new StringLocalizerExtensionMethodBuilder(input, input, anywhere);
        var method = sut.Build(targetType);

        var actual = method.Parameters.Select(x => x.Declaration).ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Hello {0}", "object _0")]
    [InlineData("Hello {name}", "object name")]
    [InlineData("Hello {0}, today is {1}", "object _0, object _1")]
    [InlineData("Hello {name}, today is {date}", "object name, object date")]
    [InlineData("{date} Hello {name}, today is {date}", "object date, object name")]
    public void Declares_Parameters_In_Signature(string input, string expectedPartialSignature)
    {
        var sut = new StringLocalizerExtensionMethodBuilder(input, input, anywhere);
        var method = sut.Build(targetType);

        var actual = method.Signature;
        var expected = $"({ExtensionMethodInfo.ThisParameterFor(targetType)}, {expectedPartialSignature})";

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello {0}", "_0")]
    [InlineData("Hello {name}", "name")]
    [InlineData("Hello {0}, today is {1}", "_0, _1")]
    [InlineData("Hello {name}, today is {date}", "name, date")]
    [InlineData("{date} Hello {name}, today is {date}", "date, name")]
    public void Passes_Parameters_In_Invocation(string input, string expectedInvocation)
    {
        var sut = new StringLocalizerExtensionMethodBuilder(input, input, anywhere);
        var method = sut.Build(targetType);

        var actual = method.Body;
        var expected = $@"that[""{input}"", {expectedInvocation}]";

        actual.Should().Be(expected);
    }
}
