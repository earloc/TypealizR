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

namespace TypealizeR.Analyzer
{
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

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var invocation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => MakeUppercaseAsync(context.Document, invocation, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> MakeUppercaseAsync(Document document, InvocationExpressionSyntax syntax, CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            var identifierNameSyntax = syntax.Expression as MemberAccessExpressionSyntax;
            var memberNameSyntax = identifierNameSyntax.Name;
            var localNameSyntax = identifierNameSyntax.Expression as IdentifierNameSyntax;
            // var newName = identifierToken.Text.ToUpperInvariant();

            var indexerAccess = $"{localNameSyntax.TryGetInferredMemberName()}["{memberNameSyntax.TryGetInferredMemberName()}"]";

            // // Get the symbol representing the type to be renamed.
            // var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            // var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // // Produce a new solution that has all references to that type renamed, including the declaration.
            // var originalSolution = document.Project.Solution;
            // var optionSet = originalSolution.Workspace.Options;
            // var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // // Return the new solution with the now-uppercase type name.
            // return newSolution;
            return document.Project.Solution;
        }
    }
}
