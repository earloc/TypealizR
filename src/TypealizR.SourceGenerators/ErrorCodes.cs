using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators;
internal static class ErrorCodes
{
	record struct DiagnosticsId (string Code, string Title);

	static string LinkToDocs(DiagnosticsId id) => $"https://github.com/earloc/TypealizR/blob/main/docs/reference/{id.Code}_{id.Title}.md";

	static readonly DiagnosticsId TR0001 = new(nameof(TR0001), "TargetProjectRootDirectoryNotFound");
	internal static Diagnostic TargetProjectRootDirectoryNotFound_0001() =>
		Diagnostic.Create(
			new(id: TR0001.Code,
				title: TR0001.Title,
				messageFormat: "The code generator could not determine the projects root-directory. See {0}",
				category: "Project",
				defaultSeverity: DiagnosticSeverity.Error,
				isEnabledByDefault: true,
				description: "The code generator could not determine the projects root-directory",
				helpLinkUri: LinkToDocs(TR0001)
			),
			Location.None, LinkToDocs(TR0001)
		);

	static readonly DiagnosticsId TR0002 = new(nameof(TR0002), "AmbigiousRessourceKey");
	internal static Diagnostic AmbigiousRessourceKey_0002(string fileName, string rawRessourceKey, int lineNumber,  string fallback) =>
		Diagnostic.Create(
			new(id: TR0002.Code,
				title: TR0002.Title,
				messageFormat: "Ressource contains the key '{0}' that would end up as a duplicate method-name. Using '{1}' as derived name for this key. See {2}",
				category: "Readability",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an ambigious ressource-key",
				helpLinkUri: LinkToDocs(TR0002)
			),
			Location.Create(fileName, 
				textSpan: new(), 
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, fallback, LinkToDocs(TR0002)
		);

	static readonly DiagnosticsId TR0003 = new(nameof(TR0003), "UnnamedGenericParameter");
	internal static Diagnostic UnnamedGenericParameter_0003(string fileName, string rawRessourceKey, int lineNumber, string parameterName) =>
		Diagnostic.Create(
			new(id: TR0003.Code,
				title: TR0003.Title,
				messageFormat: "Ressource-key '{0}' uses the generic format-parameter '{1}'. Consider to to use a more meaningful name, instead. See {2}",
				category: "Readability",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered a generic parameter",
				helpLinkUri: LinkToDocs(TR0003)
			),
			Location.Create(fileName, 
				textSpan: new(rawRessourceKey.IndexOf(parameterName), parameterName.Length), 
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, parameterName, LinkToDocs(TR0003)
		);

	static readonly DiagnosticsId TR0004 = new(nameof(TR0004), "UnrecognizedParameterType");

	internal static Diagnostic UnrecognizedParameterType_0004(string fileName, string rawRessourceKey, int lineNumber, string parameterType) =>
		Diagnostic.Create(
			new(id: TR0004.Code,
				title: TR0004.Title,
				messageFormat: "Ressource-key '{0}' uses unrecognized parameter-type '{1}'. Falling back to 'object'. See {2}",
				category: "Readability",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an unrecognized parameter-type",
				helpLinkUri: LinkToDocs(TR0004)
			),
			Location.Create(fileName,
				textSpan: new(rawRessourceKey.IndexOf(parameterType), parameterType.Length),
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, parameterType, LinkToDocs(TR0004)
		);
}
