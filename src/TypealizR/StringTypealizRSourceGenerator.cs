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
		var builder = new StringTypealizRClassBuilder(file.FullPath, severityConfig);

		foreach (var entry in file.Entries)
		{
			builder.Add(entry.Key, entry.Value, entry.Location.LineNumber);
		}

		var extensionClass = builder.Build(new(rootNamespace, file.SimpleName, Visibility.Internal), rootNamespace);

		return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), extensionClass.Diagnostics);
	}

}
