using System.Collections.Frozen;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Document = Microsoft.CodeAnalysis.Document;

namespace TypealizR.Analyzers;

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

        var memberSyntax = Root.Expression switch
        {
            MemberAccessExpressionSyntax x => x.Expression as IdentifierNameSyntax,
            //#226 [Analyzer] [UseIndexerAnalyzer] support method syntaxes
            _ => null
        };

        if (memberSyntax is null)
        {
            return source;
        }

        var parentTypeInfo = semanticModel.GetTypeInfo(memberSyntax, cancellationToken);
        if (parentTypeInfo.Type is null)
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

        if (await declarationSyntax.GetSyntaxAsync(cancellationToken) is not MethodDeclarationSyntax rootMethodDeclarationSyntax)
        {
            return source;
        }

        if (rootMethodDeclarationSyntax.Parent is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return source;
        }

        var generatedMethodName = rootMethodDeclarationSyntax.Identifier.Text;
        var localizableStringKeyConstantName = $"_{generatedMethodName}";

        var localizableStringKeyConstantDeclaration = classDeclarationSyntax.Members
            .Where(_ => _.IsKind(SyntaxKind.FieldDeclaration))
            .Select(_ => ((FieldDeclarationSyntax)_).Declaration.Variables.FirstOrDefault())
            .Where(_ => _ is not null)
            .FirstOrDefault(_ => _.Identifier.Text == localizableStringKeyConstantName);

        if (localizableStringKeyConstantDeclaration is null)
        {
            return source;
        }

        var localizableStringKey = localizableStringKeyConstantDeclaration.Initializer.Value;

        var stringLocalizerInstanceName = memberSyntax.Identifier.Text;

        var arguments = string.Join(", ", Root.ArgumentList.Arguments.Select(_ => _.ToString()).ToArray());

        var indexSignatureCode = $"""{stringLocalizerInstanceName}[{localizableStringKey}]""";
        if (!string.IsNullOrEmpty(arguments))
        {
            indexSignatureCode = $"""{stringLocalizerInstanceName}[{localizableStringKey}, {arguments}]""";
        }
        var indexSignatureSyntax = SyntaxFactory.ParseExpression(indexSignatureCode);

        var newRoot = root.ReplaceNode(Root, indexSignatureSyntax);

        return source.WithSyntaxRoot(newRoot);
    }
}
