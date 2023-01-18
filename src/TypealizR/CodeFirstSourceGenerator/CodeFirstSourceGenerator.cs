using System.Collections.Generic;
using System.IO;
using System.Linq;using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;using Microsoft.CodeAnalysis.CSharp.Syntax;using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class CodeFirstSourceGenerator : IIncrementalGenerator
{    public void Initialize(IncrementalGeneratorInitializationContext context)    {        var allInterfaces = context.CompilationProvider.Select(            (compilation, cancel) => compilation.SyntaxTrees.Select(                tree => compilation.GetSemanticModel(tree)            ).SelectMany(                semanticModel => semanticModel.SyntaxTree                    .GetRoot()                    .DescendantNodes()                    .OfType<InterfaceDeclarationSyntax>()                    .Select(                        declaration => new { Declaration = declaration, Model = semanticModel.GetDeclaredSymbol(declaration) }                    )            )        );        var markedInterfaces = allInterfaces.SelectMany(            (interfaces, cancel) => interfaces.Where(                i => i?.Model?.GetAttributes()                    .Any(x => x.AttributeClass?.Name == "CodeFirstTypealized") ?? false            )        );        context.RegisterSourceOutput(markedInterfaces.Combine(context.CompilationProvider),            (ctxt, source) =>            {                                var iface = source.Left;                var compilation = source.Right;                var members = iface                    .Declaration                    .Members;                var methods = members                    .OfType<MethodDeclarationSyntax>()                    .Select(x => new { Declaration = x, ReturnType = x.ReturnType as IdentifierNameSyntax})                    .Where(x => x.ReturnType is not null)                    .ToArray();                var properties = members                    .OfType<PropertyDeclarationSyntax>()                    .Select(x => new { Declaration = x, Type = x.Type as IdentifierNameSyntax })                    .Where(x => x.Type is not null)                    .ToArray();            });    }
}
