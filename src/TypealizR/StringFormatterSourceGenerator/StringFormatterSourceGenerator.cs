using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using TypealizR.Core;

namespace TypealizR;
public class Arguments
{
    public bool Foo { get; set;}
    public string Bar { get; set;} = "";
}

public class ScriptContext
{
    public Arguments arg { get; set; } = new();
}

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

            var fooType = compilation.GetTypeByMetadataName("Foo.Config");

            var member = fooType?.GetMembers().Where(x => x.GetAttributes().Any(y => y?.AttributeClass?.Name == nameof(CLSCompliantAttribute))).FirstOrDefault();
            var methodSyntax = member as IMethodSymbol;
            var userProvidedString = "empty";
            if (methodSyntax is not null)
            {
               var result = Execute(methodSyntax).Result;
               userProvidedString = result ?? "none";
            }

            var hasUserModeImplementation = compilation.ContainsSymbolsWithName(StringFormatterClassBuilder.TypeName, SymbolFilter.Type, cancel);
           
            var empty = new { Exists = false, HasFormatMethod = false, ExtendedTypes = Array.Empty<string>(), SupportsDateAndTimeOnly = supportsDateAndTimeOnly, UserProvidedString = userProvidedString };

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
           
            return new { Exists = true, HasFormatMethod = hasFormatMethod, ExtendedTypes = extendedTypes, SupportsDateAndTimeOnly = supportsDateAndTimeOnly, UserProvidedString = userProvidedString };
        });


        context.RegisterSourceOutput(optionsProvider
            .Combine(existingClassProvider),
            (ctxt, source) =>
            {
                var userProvidedImplementation = source.Right;
                var options = source.Left;

                AddStringFormatterClass(ctxt, options, userProvidedImplementation.Exists, userProvidedImplementation.HasFormatMethod, userProvidedImplementation.ExtendedTypes, userProvidedImplementation.SupportsDateAndTimeOnly, userProvidedImplementation.UserProvidedString);
            });
    }

    private async Task<string?> Execute(IMethodSymbol methodSymbol)
    {
        var syntaxReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxReference == null)
        {
            return null;
        }

        var syntaxNode = syntaxReference.GetSyntax();
        if (syntaxNode is not MethodDeclarationSyntax methodDeclaration)
        {
            return null;
        }

        var body = methodDeclaration.Body?.ToFullString().Replace("{", "").Replace("}", "");
        if (body == null)
        {
            return null;
        }

        try
        {
            var options = ScriptOptions.Default
                .AddReferences(typeof(Arguments).Assembly)
                .AddImports("System", "TypealizR");

            var context = new ScriptContext();
            await CSharpScript.EvaluateAsync(body, options, context);
            return context.arg.Bar;
        }
        catch (CompilationErrorException ex)
        {
            Debug.WriteLine($"Script compilation failed: {ex.Message}");
            return null;
        }
    }

    private void AddStringFormatterClass(SourceProductionContext ctxt, GeneratorOptions options, bool stringFormatterExists, bool formatMethodExists, string[] extendedTypes, bool SupportsDateAndTimeOnly, string sample)
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

        ctxt.AddSource($"{StringFormatterClassBuilder.TypeName}.g.cs", stringFormatterBuilder.Build(GetType(), sample));
    }
}
