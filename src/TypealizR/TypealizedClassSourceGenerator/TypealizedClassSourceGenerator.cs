﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Core.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class TypealizedClassSourceGenerator : ResxFileSourceGeneratorBase
{
    protected override GeneratedSourceFile GenerateSourceFileFor(
        DirectoryInfo projectDirectory,
        string rootNamespace,
        TypeModel markerType,
        Compilation compilation,
        RessourceFile file,
        IDictionary<string, DiagnosticSeverity> severityConfig,
        CancellationToken cancellationToken
    )
    {
        var builder = new TypealizedClassBuilder(file.UseParamNamesInMethodNames, markerType, $"Typealized{markerType.Name}", rootNamespace, severityConfig);

        List<Diagnostic> diagnostics = [];

        foreach (var entry in file.Entries)
        {
            var collector = new DiagnosticsCollector(file.FullPath, entry.RawKey, entry.Location.LineNumber, severityConfig);

            if (!entry.Groups.Any())
            {
                builder.WithMember(entry.Key, entry.RawKey, entry.Value, collector);
            }
            else
            {
                builder.WithGroups(entry.Key, entry.RawKey, entry.Value, entry.Groups, collector);
            }

            diagnostics.AddRange(collector.Diagnostics);
        }

        var extensionClass = builder.Build();

        return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), diagnostics);
    }
}
