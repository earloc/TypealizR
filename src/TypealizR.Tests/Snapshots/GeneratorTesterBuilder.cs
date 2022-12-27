using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterBuilder<TGenerator>
{
    internal static GeneratorTesterBuilder<TGenerator> Create(string baseDirectory, string? rootNamespace = null) => new(baseDirectory, rootNamespace);

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

    public GeneratorTesterBuilder<TGenerator> WithResxFile(string fileName)
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
            .ToArray()
        ;

        var generator = new TypealizR.StringLocalizerSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts))
            .WithUpdatedAnalyzerConfigOptions(
                new GeneratorTesterAnalyzerConfigOptionsProvider(withoutMsBuildProjectDirectory ? null: baseDirectory, projectDir, rootNamespace)
            )
        ;

        var generatorDriver = driver.RunGenerators(compilation);

        return new GeneratorTester(generatorDriver);
    }

	class GeneratorTesterAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
	{
		internal class Options : AnalyzerConfigOptions
		{
            public Options(DirectoryInfo? baseDirectory, DirectoryInfo? alternativeProjectDirectory, string rootNamespace)
            {
                if (baseDirectory is not null)
                {
                    options.Add(GeneratorOptions.MSBUILD_PROJECT_DIRECTORY, baseDirectory.FullName);
                }
                if (alternativeProjectDirectory is not null)
                {
					options.Add(GeneratorOptions.PROJECT_DIR, alternativeProjectDirectory.FullName);
				}

				options.Add(GeneratorOptions.ROOT_NAMESPACE, rootNamespace);
			}

            private readonly Dictionary<string, string> options = new ();

            public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => options.TryGetValue(key, out value);
		}


		public GeneratorTesterAnalyzerConfigOptionsProvider(DirectoryInfo? baseDirectory, DirectoryInfo? alternativeProjectDirectory, string rootNamespace)
        {
            globalOptions = new Options(baseDirectory, alternativeProjectDirectory, rootNamespace);
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
