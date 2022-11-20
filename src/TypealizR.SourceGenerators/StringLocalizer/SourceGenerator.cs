using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;

[Generator]
public partial class SourceGenerator : IIncrementalGenerator
{

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

            if (!options.ProjectDirectory.Exists)
            {
                ctxt.ReportDiagnostic( DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
                return;
            }

			foreach (var file in files)
            {
                var builder = new ClassBuilder(file.FullPath, options.SeverityConfig);

                foreach (var entry in file.Entries)
                {
                    builder.WithMethodFor(entry.Key, entry.Value, entry.Location.LineNumber);
                }

                var targetTypeName = file.SimpleName;

                var targetNamespace = FindNameSpaceOf(options.RootNamespace, file.FullPath, options.ProjectDirectory.FullName);
                var extensionClass = builder.Build(new (targetNamespace, file.SimpleName));

				ctxt.AddSource(extensionClass.FileName, extensionClass.Body);

                foreach (var diagnostic in extensionClass.Diagnostics)
                {
					ctxt.ReportDiagnostic(diagnostic);
				}
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
