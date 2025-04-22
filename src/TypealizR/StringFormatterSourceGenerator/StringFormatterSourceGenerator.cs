using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;

namespace TypealizR;
public class Arguments
{
    public bool Foo { get; set;}
    public string Bar { get; set;} = "";
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
            if (methodSyntax is not null)
            {
               var result = Execute(methodSyntax);
                Debug.WriteLine($"found: {result}");
            }




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

    private string GenerateClassWithMethod(string methodBody, string methodName, string returnType = "bool")
    {
        return $@"
            using System;
            using TypealizR;

            public static class DynamicExecutor
            {{
                public static void {methodName}(Arguments arg)
                {{
                    {methodBody}
                }}
            }}
        ";
    }

    private Assembly CompileCode(string code)
{
    var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);

    var references = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .ToList();

    var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
        "DynamicAssembly",
        [syntaxTree],
        references,
        new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    using var ms = new System.IO.MemoryStream();
    var result = compilation.Emit(ms);

    if (!result.Success)
    {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
            var errors = string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
            throw new InvalidOperationException($"Compilation failed: {errors}");
    }

    ms.Seek(0, System.IO.SeekOrigin.Begin);
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
        return Assembly.Load(ms.ToArray());
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
    }

    private string? ExecuteMethod(string methodBody)
    {
        // Generate the class and method
        var classCode = GenerateClassWithMethod(methodBody, "Execute");

        // Compile the code
        var assembly = CompileCode(classCode);

        // Get the type and method
        var type = assembly.GetType("DynamicExecutor");
        if (type == null)
        {
            throw new InvalidOperationException("DynamicExecutor type not found.");
        }

        var method = type.GetMethod("Execute");
        if (method == null)
        {
            throw new InvalidOperationException("Execute method not found.");
        }

        // Create an instance of Arguments
        var arguments = new Arguments { Foo = false };

        // Invoke the method
        method.Invoke(null, [arguments]);

        // Return the result
        return arguments.Bar;
    }

    private string? Execute(IMethodSymbol methodSymbol)
    {
        // Get the syntax reference for the method
        var syntaxReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxReference == null)
        {
            return null; // Method body not found
        }

        // Get the syntax node
        var syntaxNode = syntaxReference.GetSyntax();
        if (syntaxNode is not MethodDeclarationSyntax methodDeclaration)
        {
            return null; // Not a method declaration
        }

        // Extract the method body
        var body = methodDeclaration.Body?.ToFullString().Replace("{", "").Replace("}", "");
        if (body == null)
        {
            return null; // No body found
        }

        // Execute the method using reflection
        var result = ExecuteMethod(body);

        return result;
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
