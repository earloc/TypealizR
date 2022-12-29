﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Builder;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class StringTypealizRSourceGenerator : ResxFileSourceGeneratorBase
{
	protected override GeneratedSourceFile GenerateSourceFileFor(DirectoryInfo projectDirectory, string rootNamespace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		var markerType = new TypeModel (rootNamespace, file.SimpleName, Visibility.Internal);

		var builder = new StringTypealizRClassBuilder(markerType, $"StringTypealizR_{markerType.FullNameForClassName}", rootNamespace, severityConfig);

		var diagnostics = new List<Diagnostic>();

		foreach (var entry in file.Entries)
		{
			var collector = new DiagnosticsCollector(file.FullPath, entry.RawKey, entry.Location.LineNumber, severityConfig);

			if (!entry.Groups.Any())
			{
				builder.WithMember(entry.Key, entry.Value, collector);
			}
			else
			{
				builder.WithGroups(entry.Key, entry.Groups, entry.Value, collector);
			}

			diagnostics.AddRange(collector.Diagnostics);
		}

		var extensionClass = builder.Build();

		return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), diagnostics);
	}

}
