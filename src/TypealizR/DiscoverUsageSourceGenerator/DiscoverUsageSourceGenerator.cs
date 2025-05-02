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

        var typeProvider = context.CompilationProvider.Select((compilation, cancel) => 
        {
            var typeArguments = new HashSet<IdentifierNameSyntax>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot(cancel);

                var propertyDeclarations = root.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>();

                foreach (var property in propertyDeclarations)
                {
                    var propertyType = property.Type;
                    if (propertyType is NullableTypeSyntax nullableType)
                    {
                        propertyType = nullableType.ElementType;
                    }

                    if (propertyType is not GenericNameSyntax name)
                    {
                        continue;
                    }

                    var typeName = name.Identifier.Text;
                    if (!typeName.StartsWith("IStringLocalizer", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    foreach (var argument in name.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>())
                    {
                        typeArguments.Add(argument);
                    }
                }
            }
            return typeArguments.ToArray();
        });

        context.RegisterSourceOutput(optionsProvider.Combine(typeProvider), (ctxt, source) => 
        {
            var options = source.Left;
            if (!options.DiscoveryEnabled)
            {
                return;
            }
            var types = source.Right;

            var typeCalls = types.Select(x => $"sp.GetRequiredService<IStringLocalizer<{x.Identifier.Text}>>()").ToArray();

            var calls = string.Join(",", typeCalls);

            ctxt.AddSource("sample.g.cs", $$"""
            namespace Microsoft.Extensions.DependencyInjection
            {
                public static class TypealizRExtensions
                {
                    public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
                    {
                        return [{{calls}}]
                    }
                }
            }
            """);
        });
    }
}
