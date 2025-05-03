﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TypealizR.Diagnostics;

namespace TypealizR.Core;

public sealed class GeneratorOptions
{
    public const string msBuildProjectDirectory_BuildProperty = "build_property.msbuildprojectdirectory";
    public const string projectDir_BuildProperty = "build_property.projectdir";
    public const string rootNamespace_BuildProperty = "build_property.rootnamespace";
    public GeneratorOptions(string? projectDirectory, string rootNamespace, IDictionary<string, DiagnosticSeverity> severityConfig)
    {
        RootNamespace = rootNamespace;
        SeverityConfig = severityConfig;
        if (projectDirectory is not null)
        {
            ProjectDirectory = new DirectoryInfo(projectDirectory);
        }
    }

    public DirectoryInfo? ProjectDirectory { get; }
    public string RootNamespace { get; }

    public IDictionary<string, DiagnosticSeverity> SeverityConfig { get; }

    public static GeneratorOptions From(AnalyzerConfigOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.TryGetValue(msBuildProjectDirectory_BuildProperty, out var projectDirectory))
        {
            options.TryGetValue(projectDir_BuildProperty, out projectDirectory);
        }

        options.TryGetValue(rootNamespace_BuildProperty, out var rootNamespace);


        var severityConfig = ReadSeverityConfig(options);

        return new(
            projectDirectory: projectDirectory,
            rootNamespace: rootNamespace ?? "",
            severityConfig: severityConfig
        );
    }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "uppercase would not be valid")]
    private static Dictionary<string, DiagnosticSeverity> ReadSeverityConfig(AnalyzerConfigOptions options)
    {
        Dictionary<string, DiagnosticSeverity> severityConfig = [];

        var availableDiagnostics = Enum.GetValues(typeof(DiagnosticsId))
            .OfType<DiagnosticsId>()
            .Select(x => x.ToString()
        );

        foreach (var diagnostic in availableDiagnostics)
        {
            var key = $"dotnet_diagnostic_{diagnostic.ToLowerInvariant()}_severity";

            if (options.TryGetValue(key, out var rawValue))
            {
                severityConfig[diagnostic] = Enum.TryParse<DiagnosticSeverity>(rawValue, true, out var severity)
                    ? severity
                    : throw new InvalidOperationException($"'{key}' has invalid value '{rawValue}'");
            }
        }

        return severityConfig;
    }
}
