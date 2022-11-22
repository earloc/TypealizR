using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using TypealizR.SourceGenerators.Extensibility;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;

[Generator]
public partial class SourceGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var settings = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => Options.From(x.GlobalOptions));
		var allResxFiles = context.AdditionalTextsProvider.Where(static x => x.Path.EndsWith(".resx"));
        var monitoredFiles = allResxFiles.Collect().Select((x, cancel) => RessourceFile.From(x));
		var stringFormatterProvided = context.CompilationProvider.Select((x, cancel) => !x.ContainsSymbolsWithName(StringFormatterClassBuilder.TypeName, SymbolFilter.Type));

        context.RegisterSourceOutput(monitoredFiles
            .Combine(settings)
            .Combine(stringFormatterProvided), 
            (ctxt, source) =>
        {
            //reads horrible, but hey, that´s THE WAY
            var files = source.Left.Left;
			var options = source.Left.Right;
			var isStringFormatterProvided = source.Right;
            
			if (!options.ProjectDirectory.Exists)
            {
                ctxt.ReportDiagnostic( DiagnosticsFactory.TargetProjectRootDirectoryNotFound_0001());
                return;
            }

            var stringFormatterBuilder = new StringFormatterClassBuilder(options.RootNamespace);
            if (isStringFormatterProvided)
            {
                stringFormatterBuilder.UserModeImplementationIsProvided();
            }

            ctxt.AddSource($"{StringFormatterClassBuilder.TypeName}.g.cs", stringFormatterBuilder.Build());

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
