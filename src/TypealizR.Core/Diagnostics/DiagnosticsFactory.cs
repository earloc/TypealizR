using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using id = TypealizR.Core.Diagnostics.DiagnosticsId;

namespace TypealizR.Core.Diagnostics;

public sealed class DiagnosticsCollector
{
    private readonly DiagnosticsFactory factory;

    public DiagnosticsCollector(string filePath, string rawRessourceKey, int lineNumber, IDictionary<string, DiagnosticSeverity>? severityConfig = null) => this.factory = new(filePath, rawRessourceKey, lineNumber, severityConfig);

    private readonly List<Diagnostic> diagnostics = [];

    public void Add(Func<DiagnosticsFactory, Diagnostic> create) => diagnostics.Add(create(factory));

    public IEnumerable<Diagnostic> Diagnostics => diagnostics;
}

public sealed class DiagnosticsFactory
{
    private const string readabilityCategory = "Readability";

    private readonly string filePath;
    private readonly string rawRessourceKey;
    private readonly int lineNumber;
    private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

    public DiagnosticsFactory(string filePath, string rawRessourceKey, int lineNumber, IDictionary<string, DiagnosticSeverity>? severityConfig = null)
    {
        this.filePath = filePath;
        this.rawRessourceKey = rawRessourceKey;
        this.lineNumber = lineNumber;
        this.severityConfig = severityConfig ?? new Dictionary<string, DiagnosticSeverity>();
    }

    private DiagnosticSeverity? SeverityFor(id id) => severityConfig.TryGetValue(id.ToString(), out var value) ? value : null;

    public static readonly DiagnosticsEntry TR0001 = new(id.TR0001, "TargetProjectRootDirectoryNotFound");
    public static Diagnostic TargetProjectRootDirectoryNotFound_0001() => Diagnostic.Create(
            new(id: TR0001.Id.ToString(),
                title: TR0001.Title,
                messageFormat: Strings.TR0001_MessageFormat,
                category: "Project",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: Strings.TR0001_Description,
                helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0001)
            ),
            Location.None,
            DiagnosticsEntry.LinkToDocs(TR0001)
        );

    public static readonly DiagnosticsEntry TR0002 = new(id.TR0002, "AmbigiousRessourceKey");
    public Diagnostic AmbigiousRessourceKey_0002(string fallback) => Diagnostic.Create(
            new(id: TR0002.Id.ToString(),
                title: TR0002.Title,
                messageFormat: Strings.TR0002_MessageFormat,
                category: readabilityCategory,
                defaultSeverity: SeverityFor(TR0002.Id) ?? DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Strings.TR0002_Description,
                helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0002)
            ),
            Location.Create(filePath.Replace("\\", "/"),
                textSpan: new(),
                lineSpan: new(
                    start: new(line: lineNumber - 1, character: 0),
                    end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
                )
            ),
            rawRessourceKey, fallback, DiagnosticsEntry.LinkToDocs(TR0002)
        );

    public static readonly DiagnosticsEntry TR0003 = new(id.TR0003, "UnnamedGenericParameter");
    public Diagnostic UnnamedGenericParameter_0003(string parameterName) => Diagnostic.Create(
            new(id: TR0003.Id.ToString(),
                title: TR0003.Title,
                messageFormat: Strings.TR0003_MessageFormat,
                category: readabilityCategory,
                defaultSeverity: SeverityFor(TR0003.Id) ?? DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Strings.TR0003_Description,
                helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0003)
            ),
            Location.Create(filePath.Replace("\\", "/"),
                textSpan: new(rawRessourceKey.IndexOf(parameterName, StringComparison.Ordinal), parameterName.Length),
                lineSpan: new(
                    start: new(line: lineNumber - 1, character: 0),
                    end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
                )
            ),
            rawRessourceKey, parameterName, DiagnosticsEntry.LinkToDocs(TR0003)
        );

    public static readonly DiagnosticsEntry TR0004 = new(id.TR0004, "UnrecognizedParameterType");

    public Diagnostic UnrecognizedParameterType_0004(string parameterTypeAnnotation) => Diagnostic.Create(
            new(id: TR0004.Id.ToString(),
                title: TR0004.Title,
                messageFormat: Strings.TR0004_MessageFormat,
                category: readabilityCategory,
                defaultSeverity: SeverityFor(TR0004.Id) ?? DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Strings.TR0004_Description,
                helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0004)
            ),
            Location.Create(filePath.Replace("\\", "/"),
                textSpan: new(rawRessourceKey.IndexOf(parameterTypeAnnotation, StringComparison.Ordinal), parameterTypeAnnotation.Length),
                lineSpan: new(
                    start: new(line: lineNumber - 1, character: 0),
                    end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
                )
            ),
            rawRessourceKey, parameterTypeAnnotation, DiagnosticsEntry.LinkToDocs(TR0004)
        );

    public static readonly DiagnosticsEntry TR0005 = new(id.TR0005, "InvalidMemberName");

    public Diagnostic InvalidMemberName_0005(string source, string replacement) => Diagnostic.Create(
            new(id: TR0005.Id.ToString(),
                title: TR0005.Title,
                messageFormat: Strings.TR0005_MessageFormat,
                category: readabilityCategory,
                defaultSeverity: SeverityFor(TR0005.Id) ?? DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Strings.TR0005_Description,
                helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0005)
            ),
            Location.Create(filePath.Replace("\\", "/"),
                textSpan: new(0, rawRessourceKey.Length),
                lineSpan: new(
                    start: new(line: lineNumber - 1, character: 0),
                    end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
                )
            ),
            rawRessourceKey, source, replacement, DiagnosticsEntry.LinkToDocs(TR0005)
        );
}
