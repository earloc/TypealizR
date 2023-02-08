using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypealizR.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterBuilder<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
    internal static GeneratorTesterBuilder<TGenerator> Create(string baseDirectory, string? rootNamespace = null, string? useParamNamesInMethodNames = null) => new(baseDirectory, rootNamespace, useParamNamesInMethodNames);

    private readonly DirectoryInfo baseDirectory;
    private readonly List<FileInfo> sourceFiles = new();
    private readonly List<FileInfo> resxFiles = new();
    private readonly Dictionary<string, string> customToolNamespaces = new();
    private readonly Dictionary<string, string> useParamNamesInMethodNames = new();

    private readonly string? rootNamespace;
    private readonly string? useParamNamesInMethodNamesBuildProperty;

    public GeneratorTesterBuilder(string baseDirectory, string? rootNamespace = null, string? useParamNamesInMethodNames = null)
    {
        this.baseDirectory = new DirectoryInfo(baseDirectory);

        if (!this.baseDirectory.Exists)
        {
            throw new ArgumentException($"the specified directory {this.baseDirectory.FullName} does not exist", nameof(baseDirectory));
        }

        this.rootNamespace = rootNamespace;
        useParamNamesInMethodNamesBuildProperty = useParamNamesInMethodNames;
    }

    private bool withoutMsBuildProjectDirectory = false;
    private DirectoryInfo? projectDir = null;
    public GeneratorTesterBuilder<TGenerator> WithoutMsBuildProjectDirectory(string? butWithProjectDir = null)
    {
        withoutMsBuildProjectDirectory = true;
        if (butWithProjectDir is not null)
        {
            this.projectDir = new DirectoryInfo(butWithProjectDir);
        }
        return this;
    }

    public GeneratorTesterBuilder<TGenerator> WithSourceFile(string fileName)
    {
        var path = Path.Combine(baseDirectory.FullName, fileName);

        var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("source-file not found", fileInfo.FullName);
        }
        sourceFiles.Add(fileInfo);
        return this;
    }

    public GeneratorTesterBuilder<TGenerator> WithResxFile(
        string fileName,
        bool andDesignerFile = false,
        string? andCustomToolNamespace = null,
        string useParamNamesInMethodNames = ""
    )
    {
        var path = Path.Combine(baseDirectory.FullName, fileName);
        var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"{fileInfo.FullName} not found", fileInfo.FullName);
        }

        resxFiles.Add(fileInfo);

        if (!string.IsNullOrEmpty(andCustomToolNamespace))
        {
            customToolNamespaces.Add(fileInfo.FullName, andCustomToolNamespace);
        }

        this.useParamNamesInMethodNames.Add(fileInfo.FullName, useParamNamesInMethodNames);

        if (andDesignerFile)
        {
            WithSourceFile(fileName.Replace(".resx", ".Designer.cs"));
        }

        return this;
    }

    public IVerifiable Build()
    {
        var syntaxTrees = sourceFiles
            .Select(x => new { Path = x.FullName, Content = File.ReadAllText(x.FullName) })
            .Select(x => CSharpSyntaxTree.ParseText(x.Content, path: x.Path))
            .ToArray();

        var compilation = CSharpCompilation.Create(
            assemblyName: "TypealizR.SnapshotTests",
            syntaxTrees: syntaxTrees
        );

        var additionalTexts = resxFiles
            .Select(x => new ResxFile(x.FullName) as AdditionalText)
            .ToArray()
        ;

        var generator = new TGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts))
            .WithUpdatedAnalyzerConfigOptions(
                new GeneratorTesterOptionsProvider(
                    withoutMsBuildProjectDirectory ? null : baseDirectory,
                    projectDir,
                    rootNamespace,
                    severityConfig,
                    customToolNamespaces,
                    useParamNamesInMethodNames,
                    useParamNamesInMethodNamesBuildProperty
                )
        );

        var generatorDriver = driver.RunGenerators(compilation);

        return new GeneratorTester(generatorDriver, Path.Combine(baseDirectory.FullName, ".snapshots"));
    }

    private readonly Dictionary<DiagnosticsId, string> severityConfig = new();

    internal GeneratorTesterBuilder<TGenerator> WithSeverityConfig(DiagnosticsId id, DiagnosticSeverity severity)
        => WithSeverityConfig(id, severity.ToString());

    internal GeneratorTesterBuilder<TGenerator> WithSeverityConfig(DiagnosticsId id, string severity)
    {
        severityConfig[id] = severity;
        return this;
    }
}

