using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterBuilder
{
    internal static GeneratorTesterBuilder Create(string baseDirectory, string? rootNamespace = null) => new(baseDirectory, rootNamespace);

    private readonly DirectoryInfo baseDirectory;
    private readonly List<FileInfo> sourceFiles = new();
    private readonly List<FileInfo> resxFiles = new();
	private readonly string rootNamespace;

	public GeneratorTesterBuilder(string baseDirectory, string? rootNamespace = null)
	{
        this.baseDirectory = new DirectoryInfo(baseDirectory);

        if (!this.baseDirectory.Exists)
        {
            throw new ArgumentException($"the specified directory {this.baseDirectory.FullName} does not exist", nameof(baseDirectory));
        }

		this.rootNamespace = rootNamespace ?? "TypealizR.Tests";
	}

	public GeneratorTesterBuilder WithSourceFile(string fileName)
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

    public GeneratorTesterBuilder WithResxFile(string fileName)
    {
		var path = Path.Combine(baseDirectory.FullName, fileName);
		var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"{fileInfo.FullName} not found", fileInfo.FullName);
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

        var generator = new TypealizR.SourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts))
            .WithUpdatedAnalyzerConfigOptions(
                new GeneratorTesterAnalyzerConfigOptionsProvider(baseDirectory, rootNamespace)
            );

        var generatorDriver = driver.RunGenerators(compilation);

        return new GeneratorTester(generatorDriver);
    }

	class GeneratorTesterAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
	{

		internal class Options : AnalyzerConfigOptions
		{
            public Options(DirectoryInfo baseDirectory, string rootNamespace)
            {
                options.Add(SourceGenerator.Options.MSBUILD_PROJECT_DIRECTORY, baseDirectory.FullName);
				options.Add(SourceGenerator.Options.ROOT_NAMESPACE, rootNamespace);
			}

            private readonly Dictionary<string, string> options = new ();

            public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => options.TryGetValue(key, out value);
		}


		public GeneratorTesterAnalyzerConfigOptionsProvider(DirectoryInfo baseDirectory, string rootNamespace)
        {
            globalOptions = new Options(baseDirectory, rootNamespace);

		}
        private readonly AnalyzerConfigOptions globalOptions;
		public override AnalyzerConfigOptions GlobalOptions => globalOptions;

		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
		{
			throw new NotImplementedException();
		}

		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
		{
			throw new NotImplementedException();
		}
	}
}
