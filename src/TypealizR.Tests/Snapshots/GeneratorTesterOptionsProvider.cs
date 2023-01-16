using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly Dictionary<string, string> customToolNamespaces;
    private readonly Dictionary<string, string> useParamNamesInMethodNames;

    public GeneratorTesterOptionsProvider(
        DirectoryInfo? baseDirectory,
        DirectoryInfo? alternativeProjectDirectory,
        string? rootNamespace,
        Dictionary<DiagnosticsId, string> severityConfig,
        Dictionary<string, string> customToolNamespaces,
        Dictionary<string, string> useParamNamesInMethodNames,
        string? useParamNamesInMethodNamesBuildProperty
    )
    {
        globalOptions = new GeneratorTesterOptions(baseDirectory, alternativeProjectDirectory, rootNamespace, severityConfig, useParamNamesInMethodNamesBuildProperty);
        this.customToolNamespaces = customToolNamespaces;
        this.useParamNamesInMethodNames = useParamNamesInMethodNames;
    }

    private readonly AnalyzerConfigOptions globalOptions;

    public override AnalyzerConfigOptions GlobalOptions => globalOptions;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        throw new NotImplementedException();
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        var copy = new GeneratorTesterOptions(this.globalOptions);

        if (customToolNamespaces.ContainsKey(textFile.Path))
        {
            copy.Set(RessourceFile.CustomToolNameSpaceItemMetadata, customToolNamespaces[textFile.Path]);
        }

        if (useParamNamesInMethodNames.ContainsKey(textFile.Path))
        {
            copy.Set(RessourceFile.UseParamNamesInMethodNamesItemMetadata, useParamNamesInMethodNames[textFile.Path]);
        }
        

        return copy;
    }
}

