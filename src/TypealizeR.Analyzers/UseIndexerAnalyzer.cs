using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizeR.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseIndexerAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = nameof(UseIndexerAnalyzer);

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Hidden, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        // enable analysis of generated code, so the analyzer also sees f.e. razor-files: https://github.com/dotnet/razor/issues/7250
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {

        if (context.Node is not InvocationExpressionSyntax invocationExpression)
        {
            return;
        }

        if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
        {
            return;
        }

        var targetSymbolName = EnsureWantedSymbolName(context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression));

        if (string.IsNullOrEmpty(targetSymbolName))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation(), memberAccessExpression.Name, targetSymbolName);
        context.ReportDiagnostic(diagnostic);
    }

    const string wantedNameSpace = "Microsoft.Extensions.Localization";
    const string wantedTypeName = "IStringLocalizer";
    const string wantedTypeFullName = $"{wantedNameSpace}.{wantedTypeName}";

    private static string? EnsureWantedSymbolName(SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol is null)
        {
            return null;
        }

        var displayName = symbolInfo.Symbol switch
        {
            ILocalSymbol x => x.Type.ToDisplayString(),
            IMethodSymbol x => x.ReturnType.ToDisplayString(),
            IPropertySymbol x => x.Type.ToDisplayString(),
            IParameterSymbol x => x.Type.ToDisplayString(),
            _ => null
        };

        if (displayName is null)
        {
            return null;
        }

        var nonGenericDisplayName = displayName.Trim('?').Split('<')[0];

        return nonGenericDisplayName != wantedTypeFullName ? null : symbolInfo.Symbol.Name;
    }
}

