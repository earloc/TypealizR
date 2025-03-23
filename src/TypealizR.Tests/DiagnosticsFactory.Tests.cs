﻿using FluentAssertions;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR.Tests;

public class DiagnosticsFactory_Tests
{

    private static DiagnosticsFactory CreateSut(DiagnosticsEntry entryid, DiagnosticSeverity severity) => new("someFile.resx", "someKey", 42, new Dictionary<string, DiagnosticSeverity>() { { entryid.Id.ToString(), severity } });

    [Theory]
    [InlineData(DiagnosticSeverity.Error)]
    [InlineData(DiagnosticSeverity.Hidden)]
    [InlineData(DiagnosticSeverity.Info)]
    [InlineData(DiagnosticSeverity.Warning)]

    public void Severity_Can_Be_Configured_For_TR0002(DiagnosticSeverity expected)
    {
        var sut = CreateSut(DiagnosticsFactory.TR0002, expected);
        var diagnostic = sut.AmbigiousRessourceKey_0002("someValue");
        diagnostic.DefaultSeverity.Should().Be(expected);
    }

    [Theory]
    [InlineData(DiagnosticSeverity.Error)]
    [InlineData(DiagnosticSeverity.Hidden)]
    [InlineData(DiagnosticSeverity.Info)]
    [InlineData(DiagnosticSeverity.Warning)]

    public void Severity_Can_Be_Configured_For_TR0003(DiagnosticSeverity expected)
    {
        var sut = CreateSut(DiagnosticsFactory.TR0003, expected);
        var diagnostic = sut.UnnamedGenericParameter_0003("someKey");
        diagnostic.DefaultSeverity.Should().Be(expected);
    }

    [Theory]
    [InlineData(DiagnosticSeverity.Error)]
    [InlineData(DiagnosticSeverity.Hidden)]
    [InlineData(DiagnosticSeverity.Info)]
    [InlineData(DiagnosticSeverity.Warning)]

    public void Severity_Can_Be_Configured_For_TR0004(DiagnosticSeverity expected)
    {
        var sut = CreateSut(DiagnosticsFactory.TR0004, expected);
        var diagnostic = sut.UnrecognizedParameterType_0004("s");
        diagnostic.DefaultSeverity.Should().Be(expected);
    }
}
