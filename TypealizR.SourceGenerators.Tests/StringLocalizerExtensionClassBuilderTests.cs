using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypealizR.SourceGenerators.StringLocalizer;

namespace TypealizR.SourceGenerators.Tests;
public class StringLocalizerExtensionClassBuilderTests
{
	private static XmlLineInfo anywhere = new()
	{
		LineNumber = 0,
		LinePosition = 0
	};

	[Fact]
    public void Simple_Method_Can_Be_Generated ()
    {
        var sut = new StringLocalizerExtensionClassBuilder();

        sut.WithMethodFor("SomeKey", "SomeValue", anywhere);

        var classInfo = sut.Build(new("Name.Space", "TypeName"));

        classInfo.Methods.Should().HaveCount(1);

        var method = classInfo.Methods.First();


        var expected = new
        {
            Name = "SomeKey",
            Signature = "(this IStringLocalizer<Name.Space.TypeName> that)",
            Body = @"that[""SomeKey""]"
        };

        var actual = new
        {
            method.Name,
            method.Signature,
            method.Body
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("SomeKey", "SomeKey")]
	[InlineData("Hello, World", "Hello, World!")]
	[InlineData("Hello, World", "Hello, World?")]
	[InlineData("Hello, {planet}", "Hello, {planet}!")]
	[InlineData("Hello, {planet}", "Hello, {planet}?")]
	public void Keys_Ending_Up_To_Produce_Duplicate_MethodNames_Produce_Diagnostics(string firstKey, string duplicateKey)
    {
        string fileName = "SomeRessource.resx";

		var sut = new StringLocalizerExtensionClassBuilder();
		
		sut.WithMethodFor(firstKey, "SomeValue", new XmlLineInfo() { LineNumber = 10, LinePosition = 0 });
		sut.WithMethodFor(duplicateKey, "SomeOtherValue", new XmlLineInfo() { LineNumber = 20, LinePosition = 0 });

        var extensionClass = sut.Build(new("Name.Space", "TypeName"));

        var firstMethod = extensionClass.Methods.First();

		var expected = new Diagnostic[]
        {
            ErrorCodes.AmbigiousRessourceKey_001010(fileName, 10, firstKey, firstMethod.Name),
			ErrorCodes.AmbigiousRessourceKey_001010(fileName, 20, duplicateKey, $"{firstMethod.Name}1"),
		}
        .Select(x => x.ToString());

		var actual = extensionClass.Warnings
            .Select(x => x.ToString());

        actual.Should().BeEquivalentTo(expected);
	}
}
