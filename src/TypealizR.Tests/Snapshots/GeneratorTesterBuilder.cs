using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterBuilder
{
    internal static GeneratorTesterBuilder Create() => new();

    private List<FileInfo> sourceFiles = new();
    private List<FileInfo> resxFiles = new();

    public GeneratorTesterBuilder WithSourceFile(string fileName)
    {
        var fileInfo = new FileInfo(fileName);

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("source-file not found", fileInfo.FullName);
        }
        sourceFiles.Add(fileInfo);
        return this;
    }

    public GeneratorTesterBuilder WithResxFile(string fileName)
    {
        var fileInfo = new FileInfo(fileName);

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("resx-file not found", fileInfo.FullName);
        }

        resxFiles.Add(fileInfo);
        return this;
    }

    public IVerifiable Build()
    {
        var syntaxTrees = sourceFiles
            .Select(x => File.ReadAllText(x.FullName))
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        var compilation = CSharpCompilation.Create(
            assemblyName: "TypealizR.SnapshotTests",
            syntaxTrees: syntaxTrees
        );

        var additionalTexts = resxFiles
            .Select(x => new ResxFile(x.FullName) as AdditionalText)
            .ToArray();

        var generator = new SourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts));

        var generatorDriver = driver.RunGenerators(compilation);
    }
}
