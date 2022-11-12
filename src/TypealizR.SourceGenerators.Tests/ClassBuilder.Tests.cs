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
public class StringLocalizerExtensionClassBuilder_Tests
{
    private const string SomeFileName = "Ressource1.resx";

	[Fact]
    public void Simple_Method_Can_Be_Generated ()
    {
        var sut = new ClassBuilder(SomeFileName);

        sut.WithMethodFor("SomeKey", "SomeValue", 0);

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
		var sut = new ClassBuilder(SomeFileName);
		
		sut.WithMethodFor(firstKey, "SomeValue", 10);
		sut.WithMethodFor(duplicateKey, "SomeOtherValue", 20);

        var extensionClass = sut.Build(new("Name.Space", "TypeName"));

        var firstMethod = extensionClass.Methods.First();

		var expected = new Diagnostic[]
        {
			ErrorCodes.AmbigiousRessourceKey_0002(SomeFileName, duplicateKey, 20, $"{firstMethod.Name}1"),
		}
        .Select(x => x.ToString());

		var actual = extensionClass.Warnings
            .Select(x => x.ToString());

        actual.Should().BeEquivalentTo(expected);
	}

	[Theory]
	[InlineData("Hello {0}",
		"TR0003"
	)]
	[InlineData("Hello {0}, today is {1}",
		"TR0003",
		"TR0003"
	)]
	public void Emits_Warning_For_Generic_Parameter_Names(string input, params string[] expectedWarnings)
	{
		var sut = new ClassBuilder(SomeFileName);

        sut.WithMethodFor(input, "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName"));

		var actual = extensionClass.Warnings.Select(x => x.Id);

		actual.Should().BeEquivalentTo(expectedWarnings);
	}

	[Theory]
	[InlineData("Hello {name:xyz}",
		"TR0004"
	)]
	[InlineData("Hello {0:xyz}",
		"TR0003",
		"TR0004"
	)]
	public void Emits_Warning_For_Unrecognized_Parameter_Type(string input, params string[] expectedWarnings)
	{
		var sut = new ClassBuilder(SomeFileName);

		sut.WithMethodFor(input, "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName"));

		var actual = extensionClass.Warnings.Select(x => x.Id);

		actual.Should().BeEquivalentTo(expectedWarnings);
	}
}
