using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    private class Settings
    {
        public Settings(string? projectDirectory, string? rootNamespace, IDictionary<string, DiagnosticSeverity> severityConfig)
        {
            RootNamespace = rootNamespace ?? "";
			SeverityConfig = severityConfig;
			ProjectDirectory = new DirectoryInfo(projectDirectory);
        }

        public DirectoryInfo ProjectDirectory { get; }
        public string RootNamespace { get; }
		public IDictionary<string, DiagnosticSeverity> SeverityConfig { get; }

		public static Settings From(AnalyzerConfigOptions options)
		{
			if (!options.TryGetValue("build_property.msbuildprojectdirectory", out var projectDirectory))
			{
				options.TryGetValue("build_property.projectdir", out projectDirectory);
			}

			options.TryGetValue("build_property.rootnamespace", out var rootNamespace);

			var severityConfig = ReadSeverityConfig(options);

			return new(
				projectDirectory: projectDirectory ?? Guid.NewGuid().ToString(),
				rootNamespace: rootNamespace ?? Guid.NewGuid().ToString(),
				severityConfig: severityConfig
			);
		}

		private static IDictionary<string, DiagnosticSeverity> ReadSeverityConfig(AnalyzerConfigOptions options)
		{
			var severityConfig = new Dictionary<string, DiagnosticSeverity>();
			foreach (var diagnosticId in new[] { "TR0001", "TR0002", "TR0003", "TR0004" }) //TODO: Do not violate open-closed-principle here
			{
				if (options.TryGetValue($"dotnet_diagnostic_{diagnosticId.ToLower()}_severity", out var rawValue))
				{
					if (Enum.TryParse<DiagnosticSeverity>(rawValue, true, out var severity))
					{
						severityConfig[diagnosticId] = severity;
					}
					else
					{
						//should we better error here?
					}
				}
			}

            return severityConfig;
		}
	}

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var settings = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => Settings.From(x.GlobalOptions));

		var allResxFiles = context.AdditionalTextsProvider.Where(static x => x.Path.EndsWith(".resx"));

        var monitoredFiles = allResxFiles.Collect().Select((x, cancel) => RessourceFile.From(x));

        var input = monitoredFiles.Combine(settings);

        context.RegisterSourceOutput(input, (ctxt, source) =>
        {
            var files = source.Left;
            var options = source.Right;

            if (!options.ProjectDirectory.Exists)
            {
                ctxt.ReportDiagnostic( DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
                return;
            }

			foreach (var file in files)
            {
                var builder = new ClassBuilder(file.FullPath, options.SeverityConfig);

                foreach (var entry in file.Entries)
                {
                    builder.WithMethodFor(entry.Key, entry.Value, entry.Location.LineNumber);
                }

                var targetTypeName = file.SimpleName;

                var targetNamespace = FindNameSpaceOf(options.RootNamespace, file.FullPath, options.ProjectDirectory.FullName);
                var extensionClass = builder.Build(new (targetNamespace, file.SimpleName));

				ctxt.AddSource(extensionClass.FileName, extensionClass.Body);

                foreach (var diagnostic in extensionClass.Diagnostics)
                {
					ctxt.ReportDiagnostic(diagnostic);
				}
			}
        });
    }

	private string FindNameSpaceOf(string? rootNamespace, string resxFilePath, string projectFullPath)
    {
        var nameSpace = resxFilePath.Replace(projectFullPath, "");
        nameSpace = nameSpace.Replace(Path.GetFileName(resxFilePath), "");
        nameSpace = nameSpace.Trim('/', '\\').Replace('/', '.').Replace('\\', '.');

        if (rootNamespace == null)
        {
            return $"{rootNamespace}.{nameSpace}".Trim('.', ' ');
        }

        return $"{rootNamespace}.{nameSpace}".Trim('.', ' ');
    }
}
