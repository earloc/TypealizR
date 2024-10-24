﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Document = Microsoft.CodeAnalysis.Document;

namespace TypealizeR.Analyzers;

public class UseIndexerCodeFixer(SyntaxNode root, Diagnostic diagnostic) : ICodeFixer<InvocationExpressionSyntax>
{
    public InvocationExpressionSyntax Root => root.FindToken(diagnostic.Location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();

    public async Task<Document> CreateChangedDocumentAsync(Document source, CancellationToken cancellationToken)
    {
        if (Root is null)
        {
            return source;
        }

        if (source is null)
        {
            return source;
        }

        var semanticModel = await source.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (Root.Expression is not MemberAccessExpressionSyntax memberSyntax)
        {
            return source;
        }

        var rootSymbolInfo = semanticModel.GetSymbolInfo(Root, cancellationToken);
        if (rootSymbolInfo.Symbol is not IMethodSymbol rootMethodSymbol)
        {
            return source;
        }

        var declarationSyntax = rootMethodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (declarationSyntax is null)
        {
            return source;
        }

        if (await declarationSyntax.GetSyntaxAsync(cancellationToken) is not MethodDeclarationSyntax rootMethodSyntax)
        {
            return source;
        }

        if (rootMethodSyntax.ExpressionBody.Expression is not ElementAccessExpressionSyntax elementAccessSyntax)
        {
            return source;
        }

        var localizableStringKey = elementAccessSyntax.ArgumentList.Arguments.FirstOrDefault();
        if (localizableStringKey is null)
        {
            return source;
        }

        if (memberSyntax.Expression is not IdentifierNameSyntax parentSyntax)
        {
            return source;
        }

        var parentTypeInfo = semanticModel.GetTypeInfo(parentSyntax, cancellationToken);
        if (parentTypeInfo.Type is null)
        {
            return source;
        }

        var stringLocalizerInstanceName = parentSyntax.Identifier.Text;

        var arguments = string.Join(", ", Root.ArgumentList.Arguments.Select(_ => _.ToString()).ToArray());

        var indexSignatureCode = $"""{stringLocalizerInstanceName}[{localizableStringKey}]""";
        if (!string.IsNullOrEmpty(arguments))
        {
            indexSignatureCode = $"""{stringLocalizerInstanceName}[{localizableStringKey}, {arguments}]""";
        }
        var indexSignatureSyntax = SyntaxFactory.ParseExpression(indexSignatureCode);

        var root = await source.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root.ReplaceNode(Root, indexSignatureSyntax);

        return source.WithSyntaxRoot(newRoot);
    }
}