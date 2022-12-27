using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;
using TypealizR.Extensibility;
using TypealizR.Builder;

namespace TypealizR;

[Generator]
public partial class StringLocalizerSourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions));
		var resxFilesProvider = context.AdditionalTextsProvider.Where(static x => x.Path.EndsWith(".resx"));
		var monitoredFiles = resxFilesProvider.Collect().Select((x, cancel) => RessourceFile.From(x));
		
		context.RegisterSourceOutput(monitoredFiles
			.Combine(optionsProvider)
			.Combine(context.CompilationProvider),
			(ctxt, source) =>
			{
				//reads horrible, but hey, that´s THE WAY
				var files = source.Left.Left;
				var options = source.Left.Right;
				var compilation = source.Right;

				foreach (var file in files)
				{
					AddExtensionClassFor_IStringLocalizer(ctxt, options, compilation, file);
				}
			});
	}

	private void AddExtensionClassFor_IStringLocalizer(SourceProductionContext ctxt, GeneratorOptions options, Compilation compilation, RessourceFile file)
	{
		if (options.ProjectDirectory == null || !(options.ProjectDirectory.Exists))
		{
			ctxt.ReportDiagnostic(DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
			return;
		}

		if (!file.Entries.Any())
		{
			return;
		}

		var generatedClass = GenerateExtensionClassFor_IStringLocalizer(options.ProjectDirectory, options.RootNamespace, compilation, file, options.SeverityConfig);

		ctxt.AddSource(generatedClass.FileName, generatedClass.Content);
		foreach (var diagnostic in generatedClass.Diagnostics)
		{
			ctxt.ReportDiagnostic(diagnostic);
		}
	}

	private GeneratedSourceFile GenerateExtensionClassFor_IStringLocalizer(DirectoryInfo projectDir, string rootNamepsace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig)
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
