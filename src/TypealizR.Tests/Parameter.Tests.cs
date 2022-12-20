using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.StringLocalizer;

namespace TypealizR.Tests;

public class Parameter_Tests
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
		var collector = new DiagnosticsCollector(new("Ressource1.resx", token, 10));

		var builder = new ParameterBuilder(token);

		var sut = builder.Build(collector).Single();

		var actual = sut.Type;

		actual.Should().Be(expected);

		if (expectInvalidTypeExpression)
		{
			var warnings = collector.Entries.Select(x => x.Id);

			warnings.Should().BeEquivalentTo(new[] { DiagnosticsFactory.TR0004.Id.ToString() });
		}

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
	"Hello {date}, today is {name}",
	"object date", "object name"
	)]

	[InlineData(
	"{date} Hello {name}, today is {date}",
	"object date", "object name"
	)]

	[InlineData(
	"{name} Hello {name}, today is {date}",
	"object name", "object date"
	)]

	[InlineData(
	"{date} Hello {date}, today is {name}",
	"object date", "object name"
	)]

	[InlineData(
	"{name} Hello {date}, today is {name}",
	"object name", "object date"
	)]

	[InlineData(
	"Hello {name:s}, today is {date:d}",
	"string name", "DateOnly date"
	)]

	[InlineData(
	"{date:d} Hello {name:s}, today is {date:d}",
	"DateOnly date", "string name"
	)]

	[InlineData(
	"{date:d} Hello {name:s}, today is {date:s}",
	"DateOnly date", "string name"
	)]

	[InlineData(
	"{date:s} Hello {name:s}, today is {date:d}",
	"string date", "string name"
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
		var sut = new ParameterBuilder(input);
		var model = sut.Build(new(new("Ressource1.resx", input, 42)));
		var writer = new ParameterDeclarationWriter(model);

		var actual = writer.ToCSharp();

		actual.Should().BeEquivalentTo(expected.ToCommaDelimited());
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
	public void Declares_Parameters_In_Signature(string input, string expected)
	{
		var sut = new ParameterBuilder(input);
		var model = sut.Build(new(new("Ressource1.resx", input, 42)));
		var writer = new ParameterDeclarationWriter(model);

		var actual = writer.ToCSharp();

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
	public void Passes_Parameters_In_Invocation(string input, string expected)
	{
		var sut = new ParameterBuilder(input);
		var parameters = sut.Build(new(new("Ressource1.resx", input, 42)));

		var actual = parameters.Select(x => x.DisplayName).ToCommaDelimited();

		actual.Should().BeEquivalentTo(expected);
	}
}

