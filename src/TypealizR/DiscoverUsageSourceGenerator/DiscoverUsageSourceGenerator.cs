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
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions));

        var propertySyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider((syntax, cancel) => {
            if (syntax is not PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                return false;
            }

            var propertyType = propertyDeclarationSyntax.Type;
            if (propertyType is NullableTypeSyntax nullableType)
            {
                propertyType = nullableType.ElementType;
            }

            if (propertyType is not GenericNameSyntax name)
            {
                return false;
            }

            var typeName = name.Identifier.Text;
            if (!typeName.StartsWith("IStringLocalizer", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }, (ctxt, cancel) => {
            
            var propertyDeclarationSyntax = (PropertyDeclarationSyntax)ctxt.Node;
            var propertyType = propertyDeclarationSyntax.Type;
            if (propertyType is NullableTypeSyntax nullableType)
            {
                propertyType = nullableType.ElementType;
            }

            var genericType = (GenericNameSyntax)propertyType;

            return genericType.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>();
        });

        context.RegisterSourceOutput(optionsProvider.Combine(propertySyntaxProvider.Collect()), (ctxt, source) => 
        {
            var options = source.Left;
            if (!options.DiscoveryEnabled)
            {
                return;
            }
            var types = source.Right;

            var set = types.SelectMany(x => x).Distinct().ToArray();

            var typeCalls = set.Select(x => $"sp.GetRequiredService<IStringLocalizer<{x.Identifier.Text}>>(),").ToArray();

            var calls = typeCalls.ToMultiline("                ", trimLast: ',');

            ctxt.AddSource("TypealzR.GetRequiredLocalizersExtensions.g.cs", $$"""
            namespace Microsoft.Extensions.DependencyInjection
            {
                public static class TypealzR_GetRequiredLocalizersExtensions
                {
                    public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
                    {
                        return [
                            {{calls}}
                        ]
                    }
                }
            }
            """);
        });
    }
}
