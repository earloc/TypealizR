using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources.NetStandard;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class SourceGenerator : IIncrementalGenerator
{
    private class Settings
    {


        public Settings(string? projectFullPath, string? rootNamespace)
        {
            ProjectFullPath = projectFullPath;
            RootNamespace = rootNamespace ?? "";

            ProjectDirectory = new DirectoryInfo(ProjectFullPath);
        }

        public string? ProjectFullPath { get; }
        public DirectoryInfo ProjectDirectory { get; }
        public string RootNamespace { get; }

        public static Settings From(AnalyzerConfigOptions options)
        {
            options.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFullPath);
            options.TryGetValue("build_property.RootNamespace", out var rootNamespace);

            return new(
                projectFullPath, rootNamespace
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
                var builder = new StringLocalizerExtensionClassBuilder(options.ProjectDirectory.FullName, options.RootNamespace);

                using (var reader = ResXResourceReader.FromFileContents(File.ReadAllText(file.FullPath)))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        builder.WithMethodFor(entry.Key.ToString(), entry.Value.ToString());
                    }
                }

                var targetTypeName = file.SimpleName;

                var targetNamespace = FindNameSpaceOf(options.RootNamespace, targetTypeName, file.FullPath, options.ProjectDirectory.FullName);
                var extensionClass = builder.Build(file.SimpleName, targetNamespace);

                ctxt.AddSource(extensionClass.FileName, extensionClass.Body);
            }
        });
    }

    private string FindNameSpaceOf(string? rootNamespace, string targetTypeName, string resxFilePath, string projectFullPath)
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
