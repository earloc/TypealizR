using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR;

public abstract class ResxFileSourceGeneratorBase : IIncrementalGenerator
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
		if (options.ProjectDirectory == null || !(options.ProjectDirectory.Exists))
		{
			ctxt.ReportDiagnostic(DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
			return;
		}

		if (!file.Entries.Any())
		{
			return;
		}

		var generatedClass = GenerateSourceFileFor(options.ProjectDirectory, options.RootNamespace, compilation, file, options.SeverityConfig);

		ctxt.AddSource(generatedClass.FileName, generatedClass.Content);
		foreach (var diagnostic in generatedClass.Diagnostics)
		{
			ctxt.ReportDiagnostic(diagnostic);
		}
	}

	protected abstract GeneratedSourceFile GenerateSourceFileFor(DirectoryInfo projectDirectory, string rootNamespace, Compilation compilation, RessourceFile file, IDictionary<string, DiagnosticSeverity> severityConfig);
}
