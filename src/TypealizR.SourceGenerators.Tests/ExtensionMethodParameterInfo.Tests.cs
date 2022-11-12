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
	[InlineData("{0}", "object", false)]
	[InlineData("{1}", "object", false)]
	[InlineData("{userName}", "object", false)]
	[InlineData("{count:int}", "int", false)]
	[InlineData("{count:i}", "int", false)]
	[InlineData("{userName:string}", "string", false)]
	[InlineData("{userName:s}", "string", false)]
	[InlineData("{now:DateTime}", "DateTime", false)]
	[InlineData("{now:dt}", "DateTime", false)]
	[InlineData("{now:DateTimeOffset}", "DateTimeOffset", false)]
	[InlineData("{now:dto}", "DateTimeOffset", false)]
	[InlineData("{today:DateOnly}", "DateOnly", false)]
	[InlineData("{today:d}", "DateOnly", false)]
	[InlineData("{now:TimeOnly}", "TimeOnly", false)]
	[InlineData("{now:t}", "TimeOnly", false)]
	[InlineData("{now:wtf}", "object", true)]
	public void Parameter_Gets_Typed_As(string token, string expected, bool expectInvalidTypeExpression)
	{
		var match = StringLocalizerExtensionMethodBuilder.parameterExpression.Match(token);
		var name = match.Groups["name"].Value;
		var expression = match.Groups["expression"].Value;

		var sut = new ExtensionMethodParameterInfo(token, name, expression);

		var actual = sut.Type;

		actual.Should().Be(expected);

		if (expectInvalidTypeExpression)
		{
			sut.InvalidTypeAnnotation.Should().NotBeNullOrEmpty();
		}
	
	}
}

