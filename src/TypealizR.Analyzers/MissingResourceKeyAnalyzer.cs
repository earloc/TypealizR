using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingResourceKeyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = nameof(TypealizR.Diagnostics.DiagnosticsId.TR1010);

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingResourceKeyAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingResourceKeyAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingResourceKeyAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Localization";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        // enable analysis of generated code, so the analyzer also sees f.e. razor-files: https://github.com/dotnet/razor/issues/7250
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ElementAccessExpression);
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        
        if (context.Node is not ElementAccessExpressionSyntax elementAccessExpression)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, elementAccessExpression.GetLocation(), "Bar", "en");
        context.ReportDiagnostic(diagnostic);
    }
}

