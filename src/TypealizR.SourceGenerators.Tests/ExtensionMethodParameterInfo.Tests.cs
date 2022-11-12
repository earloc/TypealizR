using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.SourceGenerators.StringLocalizer;

namespace TypealizR.SourceGenerators.Tests;

public class ExtensionMethodParameterInfo_Tests
{
	[Theory]
	[InlineData("{0}", "object")]
	[InlineData("{1}", "object")]
	[InlineData("{userName}", "object")]
	[InlineData("{count:int}", "int")]
	[InlineData("{count:i}", "int")]
	[InlineData("{userName:string}", "string")]
	[InlineData("{userName:s}", "string")]
	[InlineData("{now:DateTime}", "DateTime")]
	[InlineData("{now:dt}", "DateTime")]
	[InlineData("{now:DateTimeOffset}", "DateTimeOffset")]
	[InlineData("{now:dto}", "DateTimeOffset")]
	[InlineData("{today:DateOnly}", "DateOnly")]
	[InlineData("{today:d}", "DateOnly")]
	[InlineData("{now:TimeOnly}", "TimeOnly")]
	[InlineData("{now:t}", "TimeOnly")]
	[InlineData("{now:wtf}", "object")]
	public void Parameter_Gets_Typed_As(string token, string expected)
	{
		var match = StringLocalizerExtensionMethodBuilder.parameterExpression.Match(token);
		var name = match.Groups["name"].Value;
		var expression = match.Groups["expression"].Value;

		var sut = new ExtensionMethodParameterInfo(token, name, expression);

		var actual = sut.Type;

		actual.Should().Be(expected);
	}
}

