using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators;
internal static class ErrorCodes
{


	internal static Diagnostic TargetProjectRootDirectoryNotFound_000010() =>
		Diagnostic.Create(
			new(id: "TYPEALIZR000010",
				title: "TargetProjectRootDirectoryNotFound",
				messageFormat: "The code generator could not determine the projects root-directory",
				category: "Project",
				defaultSeverity: DiagnosticSeverity.Error,
				isEnabledByDefault: true,
				description: "The code generator could not determine the projects root-directory"
			),
			Location.None
		);

	internal static Diagnostic AmbigiousRessourceKey_001010(string fileName, int lineNumber, string rawRessourceKey, string fallback) =>
		Diagnostic.Create(
			new(id: "TYPEALIZR001010",
				title: "AmbigiousRessourceKey",
				messageFormat: "Ressource contains the key '{0}' that would end up as a duplicate method-name. Using '{1}' as derived name for this key",
				category: "Readability",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an ambigious ressource-key"
			),
			Location.Create(fileName, 
				textSpan: new(), 
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, fallback
		);

	internal static Diagnostic UnnamedGenericParameter_001011(string fileName, int lineNumber, string rawRessourceKey, string parameterName) =>
		Diagnostic.Create(
			new(id: "TYPEALIZR001011",
				title: "UnnamedGenericParameter",
				messageFormat: "Ressource-key '{0}' uses the generic format-parameter '{1}'. Consider to to use a more meaningful name, instead",
				category: "Readability",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered a generic parameter"
			),
			Location.Create(fileName, 
				textSpan: new(rawRessourceKey.IndexOf(parameterName), parameterName.Length), 
				lineSpan: new(
					start: new(line: lineNumber - 1, character: 0),
					end: new(line: lineNumber - 1, character: rawRessourceKey.Length - 1)
				)
			),
			rawRessourceKey, parameterName
		);
}
