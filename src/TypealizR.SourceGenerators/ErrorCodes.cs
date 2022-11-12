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

	internal static Diagnostic AmbigiousRessourceKey_001010(string fileName, int lineNumber, string key, string fallback) =>
		Diagnostic.Create(
			new(id: "TYPEALIZR001010",
				title: "AmbigiousRessourceKey",
				messageFormat: "Ressource contains the key '{0}' that would end up as a duplicate method-name. Using '{1}' as dereived name for this key",
				category: "Duplicates",
				defaultSeverity: DiagnosticSeverity.Warning,
				isEnabledByDefault: true,
				description: "Encountered an ambigious ressource-key"
			),
			Location.Create(fileName, textSpan: new(), new(
					start: new(line: lineNumber, character: 0),
					end: new(line: lineNumber, character: key.Length - 1)
				)
			),
			key, fallback
		);

}
