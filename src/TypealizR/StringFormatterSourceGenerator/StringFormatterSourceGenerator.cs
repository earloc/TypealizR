using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class StringFormatterSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions));
        var existingClassProvider = context.CompilationProvider.Combine(optionsProvider).Select((providers, cancel) =>
        {
            var compilation = providers.Left;
            var options = providers.Right;

            var dateOnlyType = compilation.GetTypeByMetadataName("System.DateOnly");
            var timeOnlyType = compilation.GetTypeByMetadataName("System.TimeOnly");

            var supportsDateAndTimeOnly = dateOnlyType is not null && timeOnlyType is not null;

            var hasUserModeImplementation = compilation.ContainsSymbolsWithName(StringFormatterClassBuilder.TypeName, SymbolFilter.Type, cancel);
           
            var empty = new { Exists = false, HasFormatMethod = false, ExtendedTypes = Array.Empty<string>(), SupportsDateAndTimeOnly = supportsDateAndTimeOnly };

            if (!hasUserModeImplementation) {
                return empty;
            }
            var typeSymbol = compilation.GetTypeByMetadataName(StringFormatterClassBuilder.FullTypeName(options.RootNamespace));

            if (typeSymbol == null) {
                return empty;
            }

            var methods =  typeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.IsStatic)
                .Where(method => method.Parameters.Length == 2)
            ;

            var hasFormatMethod = methods
                .Where(method => method.Name == "Format")
                .Where(method => method.Parameters[0].Type.SpecialType == SpecialType.System_String)
                .Select(method => method.Parameters[1].Type as IArrayTypeSymbol)
                .Where(p2 => p2 != null)
                .Any(p2 => p2!.ElementType.SpecialType == SpecialType.System_Object)
            ;

            var extendMethodImplemetations = methods
                .Where(method => method.Name == "Extend")
                .Where(method => SymbolEqualityComparer.Default.Equals(method.ReturnType, method.Parameters[0].Type))
                .ToArray()
            ;

            var extendedTypes = extendMethodImplemetations.Select(method => method.ReturnType.Name).ToArray();
           
            return new { Exists = true, HasFormatMethod = hasFormatMethod, ExtendedTypes = extendedTypes, SupportsDateAndTimeOnly = supportsDateAndTimeOnly };
        });


        context.RegisterSourceOutput(optionsProvider
            .Combine(existingClassProvider),
            (ctxt, source) =>
            {
                var userProvidedImplementation = source.Right;
                var options = source.Left;

                AddStringFormatterClass(ctxt, options, userProvidedImplementation.Exists, userProvidedImplementation.HasFormatMethod, userProvidedImplementation.ExtendedTypes, userProvidedImplementation.SupportsDateAndTimeOnly);
            });
    }

    private void AddStringFormatterClass(SourceProductionContext ctxt, GeneratorOptions options, bool stringFormatterExists, bool formatMethodExists, string[] extendedTypes, bool SupportsDateAndTimeOnly)
    {
        var stringFormatterBuilder = new StringFormatterClassBuilder(options.RootNamespace, SupportsDateAndTimeOnly);
        if (stringFormatterExists)
        {
            var normalizedTypes = extendedTypes.Select(x => {
                if (ParameterAnnotation.Mappings.TryGetValue(x, out var value))
                {
                    return value;
                }
                return "";
            })
                .Where(x => !string.IsNullOrEmpty(x));

            stringFormatterBuilder.UserModeTypeExists(formatMethodExists, [.. normalizedTypes]);
        }

        ctxt.AddSource($"{StringFormatterClassBuilder.TypeName}.g.cs", stringFormatterBuilder.Build(GetType()));
    }
}
