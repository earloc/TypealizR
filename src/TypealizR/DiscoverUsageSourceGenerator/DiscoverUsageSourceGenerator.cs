using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class DiscoverUsageSourceGenerator : IIncrementalGenerator
{

    private static GenericNameSyntax? TryGetType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax is NullableTypeSyntax nullableType)
        {
            typeSyntax = nullableType.ElementType;
        }

        if (typeSyntax is not GenericNameSyntax name)
        {
            return null;
        }

        return name;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions));

        var propertySyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(DiscoverProperties, TransformProperties);
        var constructorArgumentProvider = context.SyntaxProvider.CreateSyntaxProvider(DiscoverConstructorArguments, TransformConstructorArguments);

        var providers = optionsProvider
            .Combine(propertySyntaxProvider.Collect())
            .Combine(constructorArgumentProvider.Collect())
        ;

        context.RegisterSourceOutput(providers, (ctxt, source) => 
        {
            var options = source.Left.Left;
            if (!options.DiscoveryEnabled)
            {
                return;
            }
            var constructorArgs = source.Left.Right;
            var properties = source.Right;

            var set = properties
                .SelectMany(x => x)
                .Distinct()
                .ToArray()
                .Concat(constructorArgs
                    .SelectMany(x => x)
                    .Distinct()
                    .ToArray()
                )
                .Distinct()
                .ToArray()
            ;

            var typeCalls = set.Select(type => $"sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<{type}>>(),").ToArray();

            var calls = typeCalls.ToMultiline("                ", trimLast: ',');

            ctxt.AddSource("TypealzR.GetRequiredLocalizersExtensions.g.cs", $$"""
            namespace Microsoft.Extensions.DependencyInjection
            {
                public static class TypealzR_GetRequiredLocalizersExtensions
                {
                    public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
                    {
                        return 
                        [
                            {{calls}}
                        ]
                    }
                }
            }
            """);
        });
    }

    private static bool DiscoverProperties(SyntaxNode syntax, CancellationToken cancellationToken)
    {
        if (syntax is not PropertyDeclarationSyntax declarationSyntax)
        {
            return false;
        }

        var type = TryGetType(declarationSyntax.Type);
        if (type is null)
        {
            return false;
        }

        return IsStringLocalizer(type);
    }

    private static string[] TransformProperties(GeneratorSyntaxContext ctxt, CancellationToken cancellationToken)
    {
        var declarationSyntax = (PropertyDeclarationSyntax)ctxt.Node;

        var genericType = TryGetType(declarationSyntax.Type);

        if (genericType is null)
        {
            //TODO: emit diagnostic
            return [];
        }

        return genericType
            .TypeArgumentList
            .Arguments
            .OfType<IdentifierNameSyntax>()
            .Select(x => x.Identifier.Text)
            .ToArray()
        ;
    }

    private static bool DiscoverConstructorArguments(SyntaxNode syntax, CancellationToken cancellationToken)
    {
        if (syntax is not ConstructorDeclarationSyntax declarationSyntax)
        {
            return false;
        }

        foreach (var parameter in declarationSyntax.ParameterList.Parameters)
        {
            if (parameter.Type is null) 
            {
                continue;
            }

            var type = TryGetType(parameter.Type);
            if (type is null)
            {
                continue;
            }
            if (IsStringLocalizer(type))
            {
                return true;
            }
        }

        return false;
    }

    private static string[] TransformConstructorArguments(GeneratorSyntaxContext ctxt, CancellationToken cancellationToken)
    {
        var declarationSyntax = (ConstructorDeclarationSyntax)ctxt.Node;

        var parameters = declarationSyntax
            .ParameterList
            .Parameters
            .Select(x => TryGetType(x.Type))
            .Where(IsStringLocalizer)
            .OfType<GenericNameSyntax>()
            .SelectMany(x => x.TypeArgumentList.Arguments)
            .OfType<IdentifierNameSyntax>()
            .Select(x => x.Identifier.Text)
        ;

        return parameters.ToArray();
    }

    private static bool IsStringLocalizer(GenericNameSyntax? type)
    {
        if (type is null)
        {
            return false;
        }
        var typeName = type.Identifier.Text;
        return typeName.StartsWith("IStringLocalizer", StringComparison.InvariantCultureIgnoreCase);
    }
}
