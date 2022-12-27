using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Extensibility;
using TypealizR.Builder;

namespace TypealizR;

[Generator]
public class StringLocalizerSourceGenerator : ResxFileSourceGeneratorBase
{
	protected override GeneratedSourceFile GenerateSourceFileFor(DirectoryInfo projectDir, string rootNamepsace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		var builder = new ClassBuilder(file.FullPath, severityConfig);

		foreach (var entry in file.Entries)
		{
			builder.WithMethodFor(entry.Key, entry.Value, entry.Location.LineNumber);
		}

		(var targetNamespace, var visibility) = FindNameSpaceAndVisibilityOf(compilation, rootNamepsace, file, projectDir.FullName);
		var extensionClass = builder.Build(new(targetNamespace, file.SimpleName, visibility), rootNamepsace);

		return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), extensionClass.Diagnostics);
	}

	private (string, Visibility) FindNameSpaceAndVisibilityOf(Compilation compilation, string rootNameSpace, RessourceFile resx, string projectFullPath)
	{
		var possibleMarkerTypeSymbols = compilation.GetSymbolsWithName(resx.SimpleName);
		var nameSpace = resx.FullPath.Replace(projectFullPath, "");
		nameSpace = nameSpace.Replace(Path.GetFileName(resx.FullPath), "");
		nameSpace = nameSpace.Trim('/', '\\').Replace('/', '.').Replace('\\', '.');
		if (nameSpace != rootNameSpace)
		{
			nameSpace = $"{rootNameSpace}.{nameSpace}".Trim('.');
		}

		if (!possibleMarkerTypeSymbols.Any())
		{
			return (nameSpace.Trim('.', ' '), Visibility.Internal);
		}

		var matchingMarkerType = possibleMarkerTypeSymbols.FirstOrDefault(x => x.ContainingNamespace.OriginalDefinition.ToDisplayString() == nameSpace);

		if (matchingMarkerType is null)
		{
			return (nameSpace.Trim('.', ' '), Visibility.Internal);
		}

		var visibility = (matchingMarkerType.DeclaredAccessibility == Accessibility.Public) ? Visibility.Public : Visibility.Internal;

		return (matchingMarkerType.ContainingNamespace.OriginalDefinition.ToDisplayString(), visibility);

	}
}
