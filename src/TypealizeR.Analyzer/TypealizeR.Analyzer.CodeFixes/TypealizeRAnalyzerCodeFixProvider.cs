using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Localization;

namespace TypealizeR.Analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypealizeRAnalyzerCodeFixProvider)), Shared]
public class TypealizeRAnalyzerCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(UseIndexerAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.FirstOrDefault();
        if (diagnostic == null)
        {
            return;
        }

        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var invocation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: CodeFixResources.CodeFixTitle,
                createChangedDocument: c => UseIndexSignature(context.Document, invocation, c),
                equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
            diagnostic);
    }

    private static async Task<Document> UseIndexSignature(Document document, InvocationExpressionSyntax invocationSyntax, CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        // Compute new uppercase name.
        var memberSyntax = invocationSyntax.Expression as MemberAccessExpressionSyntax;
        var memberName = memberSyntax.Name.Identifier.Text;
        var parentSyntax = memberSyntax.Expression as IdentifierNameSyntax;

        var parentTypeInfo = semanticModel.GetTypeInfo(parentSyntax, cancellationToken);

        if (parentTypeInfo.Type is null)
        {
            return document;
        }

        var parentName = parentSyntax.Identifier.Text;

        var indexSignatureCode = $"""{parentName}["{memberName}"]""";
        var indexSignatureSyntax = SyntaxFactory.ParseExpression(indexSignatureCode);

        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root.ReplaceNode(invocationSyntax, indexSignatureSyntax);

        return document.WithSyntaxRoot(newRoot);
    }
}
