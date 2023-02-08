using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR.Core;

public abstract class ResxFileSourceGeneratorBase : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        var optionsProvider = context.AnalyzerConfigOptionsProvider
            .Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions)
        );

        var resxFilesProvider = context.AdditionalTextsProvider
            .Where(static x => x.Path.EndsWith(".resx", StringComparison.Ordinal))
            .Combine(context.AnalyzerConfigOptionsProvider)
        ;

        var monitoredFiles = resxFilesProvider
            .Select((x, cancel) => new AdditionalTextWithOptions(x.Left, x.Right.GetOptions(x.Left)))
            .Collect()
            .Select(RessourceFile.From)
        ;

        context.RegisterSourceOutput(monitoredFiles
            .Combine(optionsProvider)
            .Combine(context.CompilationProvider),
            (ctxt, source) =>
            {
                var files = source.Left.Left;
                var options = source.Left.Right;
                var compilation = source.Right;

                foreach (var file in files)
                {
                    GenerateSourceFor(ctxt, options, compilation, file);
                }
            });
    }

    protected void GenerateSourceFor(SourceProductionContext ctxt, GeneratorOptions options, Compilation compilation, RessourceFile file)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        if (options.ProjectDirectory == null || !(options.ProjectDirectory.Exists))
        {
            ctxt.ReportDiagnostic(DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
            return;
        }

        if (!file.Entries.Any())
        {
            return;
        }

        (var targetNamespace, var accessability) = FindNameSpaceAndAccessabilityOf(compilation, options.RootNamespace, file, options.ProjectDirectory.FullName, ctxt.CancellationToken);
        var markerType = new TypeModel(targetNamespace, file.SimpleName, accessability);

        var generatedClass = GenerateSourceFileFor(options.ProjectDirectory, options.RootNamespace, markerType, compilation, file, options.SeverityConfig, ctxt.CancellationToken);

        ctxt.AddSource(generatedClass.FileName, generatedClass.Content);
        foreach (var diagnostic in generatedClass.Diagnostics)
        {
            ctxt.ReportDiagnostic(diagnostic);
        }
    }

    private static (string, Accessibility) FindNameSpaceAndAccessabilityOf(Compilation compilation, string rootNameSpace, RessourceFile resx, string projectFullPath, CancellationToken cancellationToken)
    {
        var possibleMarkerTypeSymbols = compilation.GetSymbolsWithName(resx.SimpleName, cancellationToken:cancellationToken).ToArray();
        var nameSpace = resx.CustomToolNamespace ?? resx.FullPath
            .Replace(projectFullPath, "")
            .Replace(Path.GetFileName(resx.FullPath), "")
            .Trim('/', '\\')
            .Replace('/', '.')
            .Replace('\\', '.');

        if (resx.CustomToolNamespace is null && nameSpace != rootNameSpace)
        {
            nameSpace = $"{rootNameSpace}.{nameSpace}".Trim('.');
        }

        if (!possibleMarkerTypeSymbols.Any())
        {
            return (nameSpace.Trim('.', ' '), Accessibility.Internal);
        }

        var matchingMarkerType = possibleMarkerTypeSymbols.FirstOrDefault(x => x.ContainingNamespace.OriginalDefinition.ToDisplayString() == nameSpace);

        if (matchingMarkerType is null)
        {
            return (nameSpace.Trim('.', ' '), Accessibility.Internal);
        }

        return (matchingMarkerType.ContainingNamespace.OriginalDefinition.ToDisplayString(), matchingMarkerType.DeclaredAccessibility);

    }

    protected abstract GeneratedSourceFile GenerateSourceFileFor(
        DirectoryInfo projectDirectory,
        string rootNamespace,
        TypeModel markerType,
        Compilation compilation,
        RessourceFile file,
        IDictionary<string, DiagnosticSeverity> severityConfig,
        CancellationToken cancellationToken
    );
}
