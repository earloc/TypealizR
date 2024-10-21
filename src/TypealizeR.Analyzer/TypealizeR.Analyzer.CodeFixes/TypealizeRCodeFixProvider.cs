using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace TypealizeR.Analyzer;

internal delegate ICodeFixer CodeFixerFactory(SyntaxNode root, Diagnostic diagnostic);

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypealizeRCodeFixProvider)), Shared]
public class TypealizeRCodeFixProvider : CodeFixProvider
{

    private readonly Dictionary<string, CodeFixerFactory> codeFixers = new()
    {
        { UseIndexerAnalyzer.DiagnosticId, (root, diagnostics) => new UseIndexerCodeFixer(root, diagnostics) }
    };

    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return [UseIndexerAnalyzer.DiagnosticId]; }
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

        if (!codeFixers.TryGetValue(diagnostic.Id, out CodeFixerFactory createCodeFixer))
        {
            return;
        }

        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: CodeFixResources.CodeFixTitle,
                createChangedDocument: c => createCodeFixer(root, diagnostic).CreateChangedDocumentAsync(context.Document, c),
                equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
            diagnostic);
    }
}
