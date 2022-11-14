using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TypealizR.SourceGenerators.StringLocalizer;

namespace TypealizR.SourceGenerators.Tests;

public class MethodBuilder_Tests
{
	private static TypeModel targetType = new("Name.Space", "TypeName");

	[Theory]
	[InlineData("Name", "Name")]
	[InlineData("Hello World!", "Hello_World")]
	[InlineData("Hello World.", "Hello_World")]
	[InlineData("Hello World?", "Hello_World")]
	[InlineData("Hello {0}", "Hello__0")]
	[InlineData("{0} Hello {0}", "Hello__0")]
	[InlineData("Hello {name}", "Hello__name")]
	[InlineData("Hello {name}, today is {now}", "Hello__name__today_is__now")]
	[InlineData("Hello {name:s}, today is {now:d}", "Hello__name__today_is__now")]
	public void Ensures_Compilable_ExtensionMethodName(string input, string expected)
	{
		var sut = new MethodBuilder(input, input, new("Ressource1.resx", input, 42));
		var method = sut.Build(targetType);

		var actual = method.Name;
		actual.Should().Be(expected);
	}

	[Theory]
	[InlineData(
		"Hello {0}",
		"object _0"
	)]
	[InlineData(
		"Hello {name}",
		"object name"
	)]
	[InlineData(
		"Hello {0}, today is {1}",
		"object _0", "object _1"
	)]
	[InlineData(
		"Hello {name}, today is {date}",
		"object name", "object date"
	)]
	[InlineData(
		"{date} Hello {name}, today is {date}",
		"object name", "object date"
	)]
	[InlineData(
		"Hello {name:s}, today is {date:d}",
		"string name", "DateOnly date"
	)]
	[InlineData(
		"{i:int} {s:string}  {dt:DateTime}  {dto:DateTimeOffset}  {d:DateOnly}  {t:TimeOnly}  {0}		   {1}          {name}",
		"int i", "string s", "DateTime dt", "DateTimeOffset dto", "DateOnly d", "TimeOnly t", "object _0", "object _1", "object name"
	)]
	[InlineData(
		"{i:i}   {s:s}       {dt:dt}        {dto:dto}             {d:d}         {t:t}",
		"int i", "string s", "DateTime dt", "DateTimeOffset dto", "DateOnly d", "TimeOnly t"
	)]

	public void Extracts_Parameters(string input, params string[] expected)
	{
		var sut = new MethodBuilder(input, input, new("Ressource1.resx", input, 42));
		var method = sut.Build(targetType);

		var actual = method.Parameters.Select(x => x.Declaration).ToArray();

		actual.Should().BeEquivalentTo(expected);
	}

	[Theory]
	[InlineData(
		"Hello {0}",
		"object _0"
	)]
	[InlineData(
		"Hello {name}",
		"object name"
	)]
	[InlineData(
		"Hello {0}, today is {1}",
		"object _0, object _1"
	)]
	[InlineData(
		"Hello {name}, today is {date}",
		"object name, object date"
	)]
	[InlineData(
		"{date} Hello {name}, today is {date}",
		"object date, object name"
	)]
	public void Declares_Parameters_In_Signature(string input, string expectedPartialSignature)
	{
		var sut = new MethodBuilder(input, input, new("Ressource1.resx", input, 42));
		var method = sut.Build(targetType);

		var actual = method.Signature;
		var expected = $"({MethodModel.ThisParameterFor(targetType)}, {expectedPartialSignature})";

		actual.Should().Be(expected);
	}

	[Theory]
	[InlineData(
		"Hello {0}",
		"_0"
	)]
	[InlineData(
		"Hello {name}",
		"name"
	)]
	[InlineData(
		"Hello {0}, today is {1}",
		"_0, _1"
	)]
	[InlineData(
		"Hello {name}, today is {date}",
		"name, date"
	)]
	[InlineData(
		"{date} Hello {name}, today is {date}",
		"date, name"
	)]
	[InlineData(
		"{0} {i:i} {s:s} {dt:dt} {dto:dto} {d:d} {t:t} {1}",
		"_0, i, s, dt, dto, d, t, _1"
	)]
	public void Passes_Parameters_In_Invocation(string input, string expectedInvocation)
	{
		var sut = new MethodBuilder(input, input, new("Ressource1.resx", input, 42));
		var method = sut.Build(targetType);

		var actual = method.Body;
		var expected = $@"that[""{input}"", {expectedInvocation}]";

		actual.Should().Be(expected);
	}

}
