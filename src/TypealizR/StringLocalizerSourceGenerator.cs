using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Builder;
using TypealizR.Core;

namespace TypealizR;

[Generator]
public sealed class StringLocalizerSourceGenerator : ResxFileSourceGeneratorBase
{
	protected override GeneratedSourceFile GenerateSourceFileFor(DirectoryInfo projectDirectory, string rootNamespace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		var builder = new ExtensionClassBuilder(file.FullPath, severityConfig);

		foreach (var entry in file.Entries)
		{
			builder.Add(entry.Key, entry.Value, entry.Location.LineNumber);
		}

		(var targetNamespace, var visibility) = FindNameSpaceAndVisibilityOf(compilation, rootNamespace, file, projectDirectory.FullName);
		var extensionClass = builder.Build(new(targetNamespace, file.SimpleName, visibility), rootNamespace);

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

		return (matchingMarkerType.ContainingNamespace.OriginalDefinition.ToDisplayString(), matchingMarkerType.DeclaredAccessibility.ToVisibilty());

	}
}
