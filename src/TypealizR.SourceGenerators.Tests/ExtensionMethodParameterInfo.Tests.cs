using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.SourceGenerators.StringLocalizer;

namespace TypealizR.SourceGenerators.Tests;

public class ExtensionMethodParameterInfo_Tests
{
	[Theory]
	[InlineData("{0}", "object2")]
	public void Parameter_Gets_Typed_As(string token, string expected)
	{
		var sut = new ExtensionMethodParameterInfo(token);

		var actual = sut.Type;

		actual.Should().Be(expected);
	}
}

