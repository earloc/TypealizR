using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Core.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class StringLocalizerExtensionsSourceGenerator : ResxFileSourceGeneratorBase
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
        var builder = new ExtensionClassBuilder(markerType, rootNamespace, file.UseParamNamesInMethodNames);

        List<Diagnostic> diagnostics = [];

        foreach (var entry in file.Entries)
        {
            var collector = new DiagnosticsCollector(file.FullPath, entry.RawKey, entry.Location.LineNumber, severityConfig);
            builder.WithExtensionMethod(entry.RawKey, entry.Value, collector);
            diagnostics.AddRange(collector.Diagnostics);
        }

        var extensionClass = builder.Build();

        return new(extensionClass.FileName, extensionClass.ToCSharp(GetType()), diagnostics);
    }


}
