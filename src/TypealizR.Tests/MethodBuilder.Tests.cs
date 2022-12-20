using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using TypealizR.StringLocalizer;

namespace TypealizR.Tests;

public class Method_Tests
{
	private static readonly TypeModel targetType = new("Name.Space", "TypeName");

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
		var sut = new MethodBuilder(input, input);
		var method = sut.Build(targetType, new(new("Ressource1.resx", input, 42)));

		var actual = method.Name;
		actual.Should().Be(expected);
	}

}
