using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Localization;

namespace TypealizeR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseIndexerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(UseIndexerAnalyzer);

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
        }


        static string desiredTargetTypeName = typeof(IStringLocalizer).FullName;
        //static string simpleTypeName = $"{nameof(Microsoft)}.{nameof(Microsoft.Extensions)}.{nameof(Microsoft.Extensions.Localization)}.{nameof(IStringLocalizer)}";

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;

            if (memberAccessExpression == null)
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression);
            var symbol = symbolInfo.Symbol as IParameterSymbol;

            if (symbol == null)
            {
                return;
            }

            if (symbol.Type.Name != nameof(IStringLocalizer))
            {
                // TODO: also check namespace?
                return;
            }

            var currentTargetTypeName = symbol.Type.ToDisplayString();

            var currentTargetNonGenericTypeName = currentTargetTypeName.Split('<')[0];

            if (currentTargetNonGenericTypeName != desiredTargetTypeName)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation(), memberAccessExpression.Name, symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}

