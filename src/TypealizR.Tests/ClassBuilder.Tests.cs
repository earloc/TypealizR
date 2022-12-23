using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypealizR.Diagnostics;
using TypealizR.Builder;
using Xunit.Sdk;

namespace TypealizR.Tests;
public class ClassBuilder_Tests
{
    private const string SomeFileName = "Ressource1.resx";
	private readonly Dictionary<string, DiagnosticSeverity> severityOverrides = new();

    [Theory]
    [InlineData("SomeKey", "SomeKey")]
	[InlineData("Hello, World", "Hello, World!")]
	[InlineData("Hello, World", "Hello, World?")]
	[InlineData("Hello, {planet}", "Hello, {planet}!")]
	[InlineData("Hello, {planet}", "Hello, {planet}?")]
	public void Keys_Ending_Up_To_Produce_Duplicate_MethodNames_Produce_Diagnostics(string firstKey, string duplicateKey)
    {
		var sut = new ClassBuilder(SomeFileName, severityOverrides);
		
		sut.WithMethodFor(firstKey, "SomeValue", 10);
		sut.WithMethodFor(duplicateKey, "SomeOtherValue", 20);

        var extensionClass = sut.Build(new("Name.Space", "TypeName"), "RootName.Space");

        var firstMethod = extensionClass.Methods.First();

		var actual = extensionClass.Diagnostics
            .Select(x => x.Id);

		var expected = new[] { DiagnosticsFactory.TR0002.Id.ToString() };

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
		var sut = new ClassBuilder(SomeFileName, severityOverrides);

        sut.WithMethodFor(input, "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName"), "RootName.Space");

		var actual = extensionClass.Diagnostics.Select(x => x.Id);

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
		var sut = new ClassBuilder(SomeFileName, severityOverrides);

		sut.WithMethodFor(input, "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName"), "RootName.Space");

		var actual = extensionClass.Diagnostics.Select(x => x.Id);

		actual.Should().BeEquivalentTo(expectedWarnings);
	}

	[Fact]
	public void DoesNot_Use_Same_Namespace_Twice()
	{
		var sut = new ClassBuilder(SomeFileName, severityOverrides);

		var extensionClass = sut.Build(new("Duplicate.Name.Space", "TypeName"), "Duplicate.Name.Space");

		var usings = extensionClass.Usings.GroupBy(x => x);

		usings.Should().AllSatisfy(x => x.Should().HaveCount(1));
	}

	[Theory]
	[InlineData(Visibility.Internal)]
	[InlineData(Visibility.Public)]
	public void Honors_Visibility(Visibility visibility)
	{
		var sut = new ClassBuilder(SomeFileName, severityOverrides);

		sut.WithMethodFor("some key", "some value", 30);

		var extensionClass = sut.Build(new("Name.Space", "TypeName", visibility), "RootName.Space");

		var actual = extensionClass.Visibility;
		var expected = visibility.ToString().ToLower();

		actual.Should().Be(expected);
	}
}
