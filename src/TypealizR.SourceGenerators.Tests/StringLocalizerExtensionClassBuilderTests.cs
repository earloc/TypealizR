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

    private const string SomeFileName = "Ressource1.resx";


	[Fact]
    public void Simple_Method_Can_Be_Generated ()
    {
        var sut = new StringLocalizerExtensionClassBuilder(SomeFileName);

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
		var sut = new StringLocalizerExtensionClassBuilder(SomeFileName);
		
		sut.WithMethodFor(firstKey, "SomeValue", 10);
		sut.WithMethodFor(duplicateKey, "SomeOtherValue", 20);

        var extensionClass = sut.Build(new("Name.Space", "TypeName"));

        var firstMethod = extensionClass.Methods.First();

		var expected = new Diagnostic[]
        {
			ErrorCodes.AmbigiousRessourceKey_001010(SomeFileName, 20, duplicateKey, $"{firstMethod.Name}1"),
		}
        .Select(x => x.ToString());

		var actual = extensionClass.Warnings
            .Select(x => x.ToString());

        actual.Should().BeEquivalentTo(expected);
	}

	[Theory]
	[InlineData("Hello {0}",
		"Ressource-key 'Hello {0}' uses the generic format-parameter '{0}'. Consider to to use a more meaningful name, instead"
	)]
	[InlineData("Hello {0}, today is {1}",
		"Ressource-key 'Hello {0}, today is {1}' uses the generic format-parameter '{0}'. Consider to to use a more meaningful name, instead",
		"Ressource-key 'Hello {0}, today is {1}' uses the generic format-parameter '{1}'. Consider to to use a more meaningful name, instead"
	)]
	public void Emits_Warning_For_Generic_Parameter_Names(string input, params string[] expectedWarnings)
	{
		var sut = new StringLocalizerExtensionClassBuilder(SomeFileName);

        sut.WithMethodFor(input, "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName"));

		var actual = extensionClass.Warnings.Select(x => x.GetMessage());

		actual.Should().BeEquivalentTo(expectedWarnings);
	}
}
