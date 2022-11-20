using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.SourceGenerators.StringLocalizer;

public partial class SourceGenerator
{
	private class Options
    {
        public Options(string? projectDirectory, string? rootNamespace, IDictionary<string, DiagnosticSeverity> severityConfig)
        {
            RootNamespace = rootNamespace ?? "";
			SeverityConfig = severityConfig;
			ProjectDirectory = new DirectoryInfo(projectDirectory);
        }

        public DirectoryInfo ProjectDirectory { get; }
        public string RootNamespace { get; }
		public IDictionary<string, DiagnosticSeverity> SeverityConfig { get; }

		public static Options From(AnalyzerConfigOptions options)
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
				var key = $"dotnet_diagnostic_{diagnosticId.ToLower()}_severity";

				if (options.TryGetValue(key, out var rawValue))
				{
					if (Enum.TryParse<DiagnosticSeverity>(rawValue, true, out var severity))
					{
						severityConfig[diagnosticId] = severity;
					}
					else
					{
						throw new Exception($"'{key}' has invalid value '{rawValue}'");
					}
				}
			}

            return severityConfig;
		}
	}
}
