﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Resources.NetStandard;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.SourceGenerators.StringLocalizer;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    private class Settings
    {


        public Settings(string? projectDirectory, string? rootNamespace)
        {
            RootNamespace = rootNamespace ?? "";

            ProjectDirectory = new DirectoryInfo(projectDirectory);
        }

        public DirectoryInfo ProjectDirectory { get; }
        public string RootNamespace { get; }

        public static Settings From(AnalyzerConfigOptions options)
        {
            options.TryGetValue("build_property.msbuildprojectdirectory", out var projectDirectory);
            options.TryGetValue("build_property.rootnamespace", out var rootNamespace);

            return new(
                projectDirectory, rootNamespace
            );
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

            foreach(var file in files)
            {

                
                var builder = new StringLocalizerExtensionClassBuilder();

                using (var reader = ResXResourceReader.FromFileContents(File.ReadAllText(file.FullPath)))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        builder.WithMethodFor(entry.Key.ToString(), entry.Value.ToString());
                    }
                }

                var targetTypeName = file.SimpleName;

                var targetNamespace = FindNameSpaceOf(options.RootNamespace, file.FullPath, options.ProjectDirectory.FullName);
                var extensionClass = builder.Build(new (targetNamespace, file.SimpleName));

                ctxt.AddSource(extensionClass.FileName, extensionClass.Body);
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