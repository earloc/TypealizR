using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;
using TypealizR.Diagnostics;using static TypealizR.Core.RessourceFile;namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class CodeFirstSourceGenerator : IIncrementalGenerator
{    private const string MarkerAttributeName = "CodeFirstTypealized";    public void Initialize(IncrementalGeneratorInitializationContext context)
    {        var optionsProvider = context.AnalyzerConfigOptionsProvider             .Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions)         );
        var allInterfaces = context.CompilationProvider.Select(
            (compilation, cancel) => compilation.SyntaxTrees.Select(
                tree => compilation.GetSemanticModel(tree)
            ).SelectMany(
                semanticModel => semanticModel.SyntaxTree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Select(
                        declaration => new { Declaration = declaration, Model = semanticModel.GetDeclaredSymbol(declaration) }
                    )                    .Where(x => x.Model is not null)                    .Select(x => new { x.Declaration, Model = x.Model! })
            )        );

        var markedInterfaces = allInterfaces.SelectMany(
            (interfaces, cancel) => interfaces.Where(
                i => i?.Model?.GetAttributes()
                    .Any(x => x.AttributeClass?.Name == MarkerAttributeName) ?? false
            )
        );

        context.RegisterSourceOutput(markedInterfaces.Combine(context.CompilationProvider).Combine(optionsProvider),
            (ctxt, source) =>
            {                var options = source.Right;
                var typealizedInterface = source.Left.Left;
                var compilation = source.Left.Right;
                var members = typealizedInterface
                    .Declaration
                    .Members;                var targetNamespace = typealizedInterface.Model.ContainingNamespace.ToDisplayParts();
                var builder = new CodeFirstClassBuilder(targetNamespace);                var diagnostics = new List<Diagnostic>();                var methods = members
                    .OfType<MethodDeclarationSyntax>()
                    .Select(x => new { Declaration = x, ReturnType = x.ReturnType as IdentifierNameSyntax})
                    .Where(x => x.ReturnType is not null)
                    .ToArray();                foreach (var method in methods)                {                    var filePath = method.Declaration.SyntaxTree.FilePath;                    var linePosition = method.Declaration.GetLocation().GetLineSpan().StartLinePosition.Line;                    var collector = new DiagnosticsCollector(filePath, method.Declaration.ToFullString(), linePosition, options.SeverityConfig);                                        //builder.WithMethod(method.Declaration);                }                var properties = members
                    .OfType<PropertyDeclarationSyntax>()
                    .Select(x => new { Declaration = x, Type = x.Type as IdentifierNameSyntax })
                    .Where(x => x.Type is not null)
                    .ToArray();                var typealizedClass = builder.Build();                var generatedFile = new GeneratedSourceFile(typealizedClass.FileName, typealizedClass.ToCSharp(), diagnostics);                foreach (var diagnostic in diagnostics)                {                    ctxt.ReportDiagnostic(diagnostic);                }                ctxt.AddSource(generatedFile.FileName, generatedFile.Content);
            });
    }
}
