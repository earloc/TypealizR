using System;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class DiscoverUsageSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctxt => {
            ctxt.AddSource("TypealizR.EnumerateLocalizersAttribute.g.cs", $$"""

            using System;

            namespace TypealizR
            {
                [AttributeUsage(AttributeTargets.Method)]
                // [GeneratedCode("TypealizR.DiscoverUsageSourceGenerator", "1.0.0")]
                public class EnumerateLocalizersAttribute : Attribute
                {
                }
            }
            """);
        });

        var methodProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "TypealizR.EnumerateLocalizersAttribute",
            predicate: static (syntax, _) => IsPartialMethodDeclarationSyntax(syntax),
            transform: static (ctxt, _) => TransformMethodDeclaration(ctxt)
        );

        var genericsProvider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (syntax, _) => DiscoverGenericNames(syntax), 
            transform: static (ctxt, _) => TransformGenericNames(ctxt)
        );

        var providers = methodProvider
            .Combine(genericsProvider.Collect())
        ;

        context.RegisterSourceOutput(providers, (ctxt, source) => 
        {
            var info = source.Left;

            if (info is null)
            {
                return;
            }
            // if (!options.DiscoveryEnabled)
            // {
            //     return;
            // }
            var generics = source.Right;

            var set = generics
                .SelectMany(x => x)
                .Distinct()
                .ToArray()
            ;

            var typeCalls = set.Select(type => $"sp.GetRequiredService<IStringLocalizer<{type}>>(),").ToArray();

            var calls = typeCalls.ToMultiline("                ", trimLast: ',');

            ctxt.AddSource($"{info.Class.FullName}.g.cs", $$"""

            using Microsoft.Extensions.DependencyInjection;
            using Microsoft.Extensions.Localization;

            namespace {{info.Class.Namespace}}
            {
                {{info.Class.Accessibility.ToVisibilty().ToString().ToLower()}} partial class {{info.Class.Name}}
                {
                    {{info.Method.Accessibility.ToVisibilty().ToString().ToLower()}} partial IStringLocalizer[] {{info.Method.Name}}(System.IServiceProvider sp)
                    {
                        return 
                        [
                            {{calls}}
                        ];
                    }
                }
            }
            """);
        });
    }

    private static bool IsPartialMethodDeclarationSyntax(SyntaxNode syntax)
    {
        if (syntax is not MethodDeclarationSyntax methodDeclaration)
        {
            return false;
        }


        var classDeclarations = methodDeclaration.Ancestors()
            .OfType<ClassDeclarationSyntax>()
        ;

        if (!classDeclarations.All(x => x.Modifiers.Any(SyntaxKind.PartialKeyword)))
        {
            return false;
        }

        if (!methodDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        if (methodDeclaration.ReturnType is not ArrayTypeSyntax arrayType)
        {
            return false;
        }

        if (arrayType.ElementType is not IdentifierNameSyntax elementTypeName)
        {
            return false;
        }

        if (!elementTypeName.Identifier.Text.EndsWith("IStringLocalizer", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return true;
    }

    internal record MethodModel(string Name, Accessibility Accessibility);
    private record MethodImplementationInfo (TypeModel Class, MethodModel Method);
    private static MethodImplementationInfo TransformMethodDeclaration(GeneratorAttributeSyntaxContext ctxt)
    {
        var methodDeclaration = (MethodDeclarationSyntax)ctxt.TargetNode;

        var classDeclaration = methodDeclaration.Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .First()
        ;

        var className = classDeclaration.Identifier.Text;

        if (className is null)
        {
            return null;
        }

         var classAccessibility = classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword) ? Accessibility.Public :
            classDeclaration.Modifiers.Any(SyntaxKind.InternalKeyword) ? Accessibility.Internal :
            classDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword) ? Accessibility.Protected :
            classDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword) ? Accessibility.Private :
            Accessibility.NotApplicable;

        var namespaceDeclaration = methodDeclaration.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
        .FirstOrDefault();

         var namespaceName = namespaceDeclaration switch
        {
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fs => fs.Name.ToString(),
            _ => null
        };

        if (namespaceName is null)
        {
            return null;
        }

       var classModel = new TypeModel (namespaceName, className, containingTypeNames: [], accessibility: classAccessibility);

        var methodAccessibility = methodDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword) ? Accessibility.Public :
            methodDeclaration.Modifiers.Any(SyntaxKind.InternalKeyword) ? Accessibility.Internal :
            methodDeclaration.Modifiers.Any(SyntaxKind.ProtectedKeyword) ? Accessibility.Protected :
            methodDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword) ? Accessibility.Private :
            Accessibility.NotApplicable;

       var methodModel = new MethodModel(methodDeclaration.Identifier.Text, methodAccessibility);

        return new (classModel, methodModel );
    }

    private static bool DiscoverGenericNames(SyntaxNode syntax)
    {
        if (syntax is NullableTypeSyntax nullableType)
        {
            syntax = nullableType.ElementType;
        }

        if (syntax is not GenericNameSyntax genericName)
        {
            return false;
        }

        var type = TryGetType(genericName);
        if (type is null)
        {
            return false;
        }

        return IsStringLocalizer(type);
    }

    private static string[] TransformGenericNames(GeneratorSyntaxContext ctxt)
    {
        var syntax = ctxt.Node;
        if (syntax is NullableTypeSyntax nullableType)
        {
            syntax = nullableType.ElementType;
        }

        if (syntax is not GenericNameSyntax genericName)
        {
            return [];
        }

        return genericName
            .TypeArgumentList
            .Arguments
            .OfType<IdentifierNameSyntax>()
            .Select(x => FullyQualifiedName(ctxt, x))
            .ToArray()
        ;
    }

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

    private static bool IsStringLocalizer(GenericNameSyntax? type)
    {
        if (type is null)
        {
            return false;
        }
        var typeName = type.Identifier.Text;
        return typeName.StartsWith("IStringLocalizer", StringComparison.InvariantCultureIgnoreCase);
    }

    private static string FullyQualifiedName(GeneratorSyntaxContext ctxt, IdentifierNameSyntax x)
    {
        var symbol = ctxt.SemanticModel.GetSymbolInfo(x).Symbol as INamedTypeSymbol;
        return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? x.Identifier.Text;
    }
}
