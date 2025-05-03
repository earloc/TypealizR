﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal sealed class GeneratorTesterOptions : AnalyzerConfigOptions
{

    private readonly Dictionary<string, string> options = [];

    public GeneratorTesterOptions(AnalyzerConfigOptions source)
    {
        foreach (var key in source.Keys)
        {
            if (source.TryGetValue(key, out var value))
            {
                options.Add(key, value);
            }
        }
    }

    internal static GeneratorTesterOptions Empty = new();

    private GeneratorTesterOptions()
    {
    }

    public GeneratorTesterOptions(
        DirectoryInfo? baseDirectory,
        DirectoryInfo? alternativeProjectDirectory,
        string? rootNamespace,
        Dictionary<DiagnosticsId, string> severityConfig,
        string? useParamNamesInMethodNames = null
    )
    {
        if (baseDirectory is not null)
        {
            options.Add(Core.GeneratorOptions.msBuildProjectDirectory_BuildProperty, baseDirectory.FullName);
        }

        if (alternativeProjectDirectory is not null)
        {
            options.Add(Core.GeneratorOptions.projectDir_BuildProperty, alternativeProjectDirectory.FullName);
        }

        if (rootNamespace is not null)
        {
            options.Add(Core.GeneratorOptions.rootNamespace_BuildProperty, rootNamespace);
        }

        foreach (var severityOverride in severityConfig)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            options.Add($"dotnet_diagnostic_{severityOverride.Key.ToString().ToLowerInvariant()}_severity", severityOverride.Value.ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        options.Add(RessourceFile.UseParamNamesInMethodNamesBuildProperty, useParamNamesInMethodNames is not null ? useParamNamesInMethodNames : "");
    }

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => options.TryGetValue(key, out value);

    public void Set(string key, string value) => options[key] = value;

    public override IEnumerable<string> Keys => options.Keys;
}

