using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTesterOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly Dictionary<string, string> customToolNamespaces;

    public GeneratorTesterOptionsProvider(
        DirectoryInfo? baseDirectory,
        DirectoryInfo? alternativeProjectDirectory,
        string? rootNamespace,
        Dictionary<DiagnosticsId, string> severityConfig,
        Dictionary<string, string> customToolNamespaces
    )
    {
        globalOptions = new GeneratorTesterOptions(baseDirectory, alternativeProjectDirectory, rootNamespace, severityConfig);
        this.customToolNamespaces = customToolNamespaces;
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
            copy.Set(RessourceFile.CustomToolNameSpaceProperty, customToolNamespaces[textFile.Path]);
        }

        return copy;
    }
}

