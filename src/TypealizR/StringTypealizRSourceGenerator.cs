using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Builder;
using TypealizR.Core;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class StringTypealizRSourceGenerator : ResxFileSourceGeneratorBase
{
	protected override GeneratedSourceFile GenerateSourceFileFor(DirectoryInfo projectDirectory, string rootNamespace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		var targetType = new TypeModel (rootNamespace, file.SimpleName, Visibility.Internal);

		var builder = new StringTypealizRClassBuilder($"StringTypealizR_{targetType.FullNameForClassName}", file.FullPath, severityConfig);

		foreach (var entry in file.Entries)
		{
			if (!entry.Groups.Any())
			{
				builder.WithMember(entry.Key, entry.Value, entry.Location.LineNumber);
			}
			else
			{
				builder.WithGroups(entry.Key, entry.Groups, entry.Value, entry.Location.LineNumber);
			}
		}

		var extensionClass = builder.Build(targetType, rootNamespace);

		return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), extensionClass.Diagnostics);
	}

}
