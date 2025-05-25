using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class CodeFirstSourceGenerator : IIncrementalGenerator
{
    private const string MarkerAttributeName = "CodeFirstTypealized";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var allInterfaces = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, cancel) => node is InterfaceDeclarationSyntax,
            static (node, cancel) => new { Declaration = (InterfaceDeclarationSyntax)node.Node, Model = node.SemanticModel }
        );

        var markedInterfaces = allInterfaces
            .Select((x, cancel) => new { x.Declaration, Model = x.Model.GetDeclaredSymbol(x.Declaration, cancel) })
            .Where(x => x.Model is not null)
            .Select((x, cancel) => new { x.Declaration, Model = x.Model! })
            .Where(x => x.Model
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name.StartsWith(MarkerAttributeName, StringComparison.Ordinal) ?? false)
            // #236: getting customized implemention name would go here (probably ;))
            )
        ;

        var optionsProvider = context.AnalyzerConfigOptionsProvider
            .Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions)
        );

        context.RegisterSourceOutput(markedInterfaces.Combine(optionsProvider),
            (ctxt, source) =>
            {
                var options = source.Right;
                var typealizedInterface = source.Left;

                var containingTypeNames = typealizedInterface.Model.ContainingType.GetContainingTypesRecursive();

                var members = typealizedInterface
                    .Declaration
                    .Members;

                var builder = new CodeFirstClassBuilder(new TypeModel(
                    typealizedInterface.Model.ContainingNamespace.ToDisplayString(),
                    typealizedInterface.Declaration.Identifier.Text,
                    containingTypeNames,
                    typealizedInterface.Declaration.Modifiers.InferrAccessibility()
                ), containingTypeNames);

                List<Diagnostic> diagnostics = [];

                TryAddMethods(builder, diagnostics, members, options, ctxt.CancellationToken);
                TryAddProperties(builder, diagnostics, members, options, ctxt.CancellationToken);

                var typealizedClass = builder.Build();
                var generatedFile = new GeneratedSourceFile(typealizedClass.FileName, typealizedClass.ToCSharp(GetType()), diagnostics);

                ctxt.AddSource(generatedFile.FileName, generatedFile.Content);
            });
    }



    private static void TryAddMethods(CodeFirstClassBuilder builder, List<Diagnostic> diagnostics, SyntaxList<MemberDeclarationSyntax> members, GeneratorOptions options, CancellationToken cancellationToken)
    {
        var methods = members
            .OfType<MethodDeclarationSyntax>()
            .Select(x => new { Declaration = x, ReturnType = x.ReturnType as IdentifierNameSyntax })
            .Where(x => x.ReturnType is not null)
            .Select(x => new { x.Declaration, ReturnType = x.ReturnType! })
            .ToArray()
        ;

        foreach (var method in methods.Select(x => x.Declaration))
        {
            var filePath = method.SyntaxTree.FilePath;
            var linePosition = method.GetLocation().GetLineSpan().StartLinePosition.Line;
            var collector = new DiagnosticsCollector(filePath, method.ToFullString(), linePosition, options.SeverityConfig);

            var defaultValue = TryGetDefaultValueFrom(method, cancellationToken);

            var methodBuilder = builder.WithMethod(method.Identifier.Text, defaultValue.Value, defaultValue.Remarks);

            foreach (var parameter in method.ParameterList.Parameters)
            {
                methodBuilder.WithParameter(parameter.Identifier.Text, parameter.Type?.ToString() ?? "object");
            }

            diagnostics.AddRange(collector.Diagnostics);
        }
    }

    private static void TryAddProperties(CodeFirstClassBuilder builder, List<Diagnostic> diagnostics, SyntaxList<MemberDeclarationSyntax> members, GeneratorOptions options, CancellationToken cancellationToken)
    {
        var properties = members
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => new { Declaration = x, Type = x.Type as IdentifierNameSyntax })
            .Where(x => x.Type is not null)
            .ToArray()
        ;

        foreach (var property in properties.Select(x => x.Declaration))
        {
            var filePath = property.SyntaxTree.FilePath;
            var linePosition = property.GetLocation().GetLineSpan().StartLinePosition.Line;
            var collector = new DiagnosticsCollector(filePath, property.ToFullString(), linePosition, options.SeverityConfig);
            var name = property.Identifier.Text;

            var defaultValue = TryGetDefaultValueFrom(property, cancellationToken);

            builder.WithProperty(name, defaultValue.Value, defaultValue.Remarks);

            diagnostics.AddRange(collector.Diagnostics);
        }
    }

    private static (string Value, string Remarks) TryGetDefaultValueFrom(SyntaxNode declaration, CancellationToken cancellationToken)
    {
        var allTrivias = declaration.GetLeadingTrivia().Where(x => x.HasStructure).ToArray();

        if (allTrivias.Length == 0)
        {
            var tree = CSharpSyntaxTree.ParseText(declaration.ToFullString(), cancellationToken: cancellationToken);
            allTrivias = tree.GetCompilationUnitRoot(cancellationToken).GetLeadingTrivia().Where(x => x.HasStructure).ToArray();
        }

        var documentation = Array.Find(allTrivias, x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

        if (documentation.GetStructure() is not DocumentationCommentTriviaSyntax structure)
        {
            return default;
        }

        var xmlTagComments = structure.Content
            .OfType<XmlElementSyntax>()
            .Select(x => new
            {
                Name = x.StartTag.ToString(),
                Tag = x
            })
            .ToArray()
        ;

        var summary = xmlTagComments.FirstOrDefault(x => x.Name == "<summary>");
        if (summary is null)
        {
            return default;
        }

        var remarks = xmlTagComments.FirstOrDefault(x => x.Name == "<remarks>")?.Tag.Content.ToString() ?? "";

        var xmlComments = summary.Tag.Content.Where(x => x is XmlTextSyntax or XmlEmptyElementSyntax).ToArray();

        var builder = new StringBuilder();

        foreach (var xmlNodeSyntax in xmlComments)
        {
            var value = xmlNodeSyntax switch
            {
                XmlTextSyntax x => x.TextTokens
                    .Select(_ => _.Text)
                    .Join(),

                XmlEmptyElementSyntax x => x.Attributes
                    .OfType<XmlNameAttributeSyntax>()
                    .Select(a => $$"""{{{a.Identifier.Identifier.ValueText}}}""")
                    .ToCommaDelimited(),

                _ => ""
            };

            if (!string.IsNullOrEmpty(value))
            {
                builder.Append(value);
            }
        }

        //TODO: https://github.com/earloc/TypealizR/issues/277 optimize: reading (multiline) remarks
        remarks = remarks.Replace('\n', ' ');
        remarks = remarks.Replace('/', ' ');
        remarks = remarks.Trim();

        return (builder.ToString().Trim().Escape(), remarks);
    }
}
