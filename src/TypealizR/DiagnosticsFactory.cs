using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using id = TypealizR.SourceGenerators.DiagnosticsId;

namespace TypealizR.SourceGenerators;
internal class DiagnosticsFactory
{
	private readonly string filePath;
	private readonly string rawRessourceKey;
	private readonly int lineNumber;
	private readonly IDictionary<string, DiagnosticSeverity> severityMap;

	public DiagnosticsFactory(string filePath, string rawRessourceKey, int lineNumber, IDictionary<string, DiagnosticSeverity>? severityMap = null)
	{
		this.filePath = filePath;
		this.rawRessourceKey = rawRessourceKey;
		this.lineNumber = lineNumber;
		this.severityMap = severityMap ?? new Dictionary<string, DiagnosticSeverity>();
	}

	private DiagnosticSeverity? SeverityFor(DiagnosticsId id) => severityMap.ContainsKey(id.ToString()) ? severityMap[id.ToString()] : null;

	internal static readonly DiagnosticsEntry TR0001 = new(id.TR0001, "TargetProjectRootDirectoryNotFound");
	internal static Diagnostic TargetProjectRootDirectoryNotFound_0001() =>
		Diagnostic.Create(
			new(id: TR0001.Id.ToString(),
				title: TR0001.Title,
				messageFormat: "The code generator could not determine the projects root-directory. See {0}",
				category: "Project",
				defaultSeverity: DiagnosticSeverity.Error,
				isEnabledByDefault: true,
				description: "The code generator could not determine the projects root-directory",
				helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0001)
			),
			Location.None, DiagnosticsEntry.LinkToDocs(TR0001)
		);

	internal static readonly DiagnosticsEntry TR0002 = new(id.TR0002, "AmbigiousRessourceKey");
	internal Diagnostic AmbigiousRessourceKey_0002(string fallback) =>
		Diagnostic.Create(
			new(id: TR0002.Id.ToString(),
				title: TR0002.Title,
				messageFormat: "Ressource contains the key '{0}' that would end up as a duplicate method-name. Using '{1}' as derived name for this key. See {2}",
				category: "Readability",
				defaultSeverity: SeverityFor(TR0002.Id) ?? DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an ambigious ressource-key",
				helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0002)
			),
			Location.Create(filePath,
				textSpan: new(),
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, fallback, DiagnosticsEntry.LinkToDocs(TR0002)
		);

	internal static readonly DiagnosticsEntry TR0003 = new(id.TR0003, "UnnamedGenericParameter");
	internal Diagnostic UnnamedGenericParameter_0003(string parameterName) =>
		Diagnostic.Create(
			new(id: TR0003.Id.ToString(),
				title: TR0003.Title,
				messageFormat: "Ressource-key '{0}' uses the generic format-parameter '{1}'. Consider to use a more meaningful name, instead. See {2}",
				category: "Readability",
				defaultSeverity: SeverityFor(TR0003.Id) ?? DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered a generic parameter",
				helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0003)
			),
			Location.Create(filePath,
				textSpan: new(rawRessourceKey.IndexOf(parameterName), parameterName.Length),
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, parameterName, DiagnosticsEntry.LinkToDocs(TR0003)
		);

	internal static readonly DiagnosticsEntry TR0004 = new(id.TR0004, "UnrecognizedParameterType");

	internal Diagnostic UnrecognizedParameterType_0004(string parameterTypeAnnotation) =>
		Diagnostic.Create(
			new(id: TR0004.Id.ToString(),
				title: TR0004.Title,
				messageFormat: "Ressource-key '{0}' uses unrecognized parameter-annotation '{1}'. Falling back to 'object'. See {2}",
				category: "Readability",
				defaultSeverity: SeverityFor(TR0004.Id) ?? DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an unrecognized parameter-type",
				helpLinkUri: DiagnosticsEntry.LinkToDocs(TR0004)
			),
			Location.Create(filePath,
				textSpan: new(rawRessourceKey.IndexOf(parameterTypeAnnotation), parameterTypeAnnotation.Length),
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, parameterTypeAnnotation, DiagnosticsEntry.LinkToDocs(TR0004)
		);
}
