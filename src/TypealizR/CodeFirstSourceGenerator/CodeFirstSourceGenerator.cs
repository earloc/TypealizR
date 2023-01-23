using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;
using TypealizR.Diagnostics;
using static TypealizR.Core.RessourceFile;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class CodeFirstSourceGenerator : IIncrementalGenerator
{
    private const string MarkerAttributeName = "CodeFirstTypealized";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider
             .Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions)
         );

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
                    )
                    .Where(x => x.Model is not null)
                    .Select(x => new { x.Declaration, Model = x.Model! })
            )
        );

        var markedInterfaces = allInterfaces.SelectMany(
            (interfaces, cancel) => interfaces.Where(
                i => i?.Model?.GetAttributes()
                    .Any(x => x.AttributeClass?.Name == MarkerAttributeName) ?? false
            )
        );

        context.RegisterSourceOutput(markedInterfaces.Combine(context.CompilationProvider).Combine(optionsProvider),
            (ctxt, source) =>
            {
                var options = source.Right;
                var typealizedInterface = source.Left.Left;
                var compilation = source.Left.Right;
                var members = typealizedInterface
                    .Declaration
                    .Members;

                var builder = new CodeFirstClassBuilder(new TypeModel(
                    typealizedInterface.Model.ContainingNamespace.ToDisplayString(),
                    typealizedInterface.Declaration.Identifier.Text
                ));

                var diagnostics = new List<Diagnostic>();

                TryAddMethods(builder, diagnostics, members, options);

                TryAddProperties(builder, diagnostics, members, options);

                var typealizedClass = builder.Build();
                var generatedFile = new GeneratedSourceFile(typealizedClass.FileName, typealizedClass.ToCSharp(GetType()), diagnostics);

                foreach (var diagnostic in diagnostics)
                {
                    ctxt.ReportDiagnostic(diagnostic);
                }

                ctxt.AddSource(generatedFile.FileName, generatedFile.Content);
            });
    }

    private void TryAddMethods(CodeFirstClassBuilder builder, List<Diagnostic> diagnostics, SyntaxList<MemberDeclarationSyntax> members, GeneratorOptions options)
    {
        var methods = members
            .OfType<MethodDeclarationSyntax>()
            .Select(x => new { Declaration = x, ReturnType = x.ReturnType as IdentifierNameSyntax })
            .Where(x => x.ReturnType is not null)
            .Select(x => new { x.Declaration, ReturnType = x.ReturnType! })
            .ToArray()
        ;

        foreach (var method in methods)
        {
            var filePath = method.Declaration.SyntaxTree.FilePath;
            var linePosition = method.Declaration.GetLocation().GetLineSpan().StartLinePosition.Line;
            var collector = new DiagnosticsCollector(filePath, method.Declaration.ToFullString(), linePosition, options.SeverityConfig);

            var name = method.Declaration.Identifier.Text;
            var defaultValue = TryGetDefaultValueFrom(method.Declaration);

            var methodBuilder = builder.WithMethod(method.Declaration.Identifier.Text, defaultValue);

            foreach (var parameter in method.Declaration.ParameterList.Parameters)
            {
                methodBuilder.WithParameter(parameter.Identifier.Text, parameter.Type.ToString());
            }

            diagnostics.AddRange(collector.Diagnostics);
        }
    }

    private void TryAddProperties(CodeFirstClassBuilder builder, List<Diagnostic> diagnostics, SyntaxList<MemberDeclarationSyntax> members, GeneratorOptions options)
    {
        var properties = members
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => new { Declaration = x, Type = x.Type as IdentifierNameSyntax })
            .Where(x => x.Type is not null)
            .ToArray()
        ;

        foreach (var property in properties)
        {
            var filePath = property.Declaration.SyntaxTree.FilePath;
            var linePosition = property.Declaration.GetLocation().GetLineSpan().StartLinePosition.Line;
            var collector = new DiagnosticsCollector(filePath, property.Declaration.ToFullString(), linePosition, options.SeverityConfig);
            var name = property.Declaration.Identifier.Text;

            var defaultValue = TryGetDefaultValueFrom(property.Declaration);

            builder.WithProperty(name, defaultValue);

            diagnostics.AddRange(collector.Diagnostics);
        }
    }

    private string? TryGetDefaultValueFrom(SyntaxNode declaration)
    {
        if (!declaration.HasStructuredTrivia)
        {
            return default;
        }

        var trivia = declaration.GetLeadingTrivia();

        var documentation = trivia.FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        var structure = documentation.GetStructure() as DocumentationCommentTriviaSyntax;

        if (structure is null)
        {
            return default; 
        }

        var comment = structure.Content.OfType< XmlElementSyntax>().FirstOrDefault();

        if (comment is null)
        {
            return default;
        }

        var xmlComments = comment.Content.Where(x => x.GetType() == typeof(XmlTextSyntax) || x.GetType() == typeof(XmlEmptyElementSyntax)).ToArray();

        var builder = new StringBuilder();

        foreach (var xmlNodeSyntax in xmlComments)
        {
            var value = xmlNodeSyntax switch
            {
                (XmlTextSyntax x) => x.TextTokens
                    .Select(_ => _.Text.Trim())
                    .Where(_ => !string.IsNullOrEmpty(_))
                    .ToSpaceDelimited() ?? "",

                (XmlEmptyElementSyntax x) => x.Attributes
                    .OfType<XmlNameAttributeSyntax>()
                    .Select(a => $$"""{{{a.Identifier.Identifier.ValueText}}}""")
                    .ToCommaDelimited(),

                _ => ""
            };

            if (!string.IsNullOrEmpty(value))
            {
                builder.Append(value);
                builder.Append(" ");
            }
        }

        return builder.ToString().Trim();
    }
}
