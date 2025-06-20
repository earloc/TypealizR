﻿using TypealizR.Core;
using TypealizR.Core.Diagnostics;
using TypealizR.Extensions;

namespace TypealizR.Tests;

public class ParameterBuilder_Tests
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
        var collector = new DiagnosticsCollector("Ressource1.resx", token, 10);

        var builder = new ParameterBuilder(token);

        var sut = builder.Build(collector).Single();

        var actual = sut.Type;

        actual.ShouldBe(expected);

        if (expectInvalidTypeExpression)
        {
            var warnings = collector.Diagnostics.Select(x => x.Id).ToArray();

            warnings.ShouldBeEquivalentTo(new[] { DiagnosticsFactory.TR0004.Id.ToString() });
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
    [InlineData(
    "{i:i@x} {s:s@x}     {dt:dt@x}      {dto:dto@x}           {d:d@x}       {t:t@x}",
    "int i", "string s", "DateTime dt", "DateTimeOffset dto", "DateOnly d", "TimeOnly t"
    )]
    public void Extracts_Parameters_Declaration(string input, params string[] expected)
    {
        var sut = new ParameterBuilder(input);
        var model = sut.Build(new("Ressource1.resx", input, 42));

        var actual = model.ToDeclarationCSharp();

        actual.ShouldBeEquivalentTo(expected.ToCommaDelimited());
    }

    [Theory]
    [InlineData(
    "{i:i@x} {s:s@x} {dt:dt@x} {dto:dto@x} {d:d@x} {t:t@x}",
    "i",     "s",    "dt",     "dto",      "d",    "t"
    )]
    public void Extracts_Parameters_Names(string input, params string[] expected)
    {
        var sut = new ParameterBuilder(input);
        var model = sut.Build(new("Ressource1.resx", input, 42));

        var actual = model.Select(x => x.Name).ToCommaDelimited();

        actual.ShouldBeEquivalentTo(expected.ToCommaDelimited());
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
        var model = sut.Build(new("Ressource1.resx", input, 42));

        var actual = model.ToDeclarationCSharp();

        actual.ShouldBe(expected);
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
    [InlineData(
        "Hello {name:string@x}",
        "name"
    )]
    public void Passes_Parameters_In_Invocation(string input, string expected)
    {
        var sut = new ParameterBuilder(input);
        var parameters = sut.Build(new("Ressource1.resx", input, 42));

        var actual = parameters.Select(x => x.DisplayName).ToCommaDelimited();

        actual.ShouldBeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("{argumentName@ex}")]
    [InlineData("{argumentName:string@ex}")]
    [InlineData("{argumentName:s@ex}")]
    [InlineData("{argumentName:int@ex}")]
    [InlineData("{argumentName:i@ex}")]
    [InlineData("{argumentName:DateTime@ex}")]
    [InlineData("{argumentName:dt@ex}")]
    [InlineData("{argumentName:DateTimeOffset@ex}")]
    [InlineData("{argumentName:dto@ex}")]
    [InlineData("{argumentName:DateOnly@ex}")]
    [InlineData("{argumentName:d@ex}")]
    [InlineData("{argumentName:TimeOnly@ex}")]
    [InlineData("{argumentName:t@ex}")]
    public void Extracts_Annotation_Extensions(string input)
    {
        var sut = new ParameterBuilder(input);
        var parameters = sut.Build(new("Ressource1.resx", input, 42));

        var actual = parameters.Select(x => x.Extension).ToCommaDelimited();

        actual.ShouldBeEquivalentTo("ex");
    }
}

