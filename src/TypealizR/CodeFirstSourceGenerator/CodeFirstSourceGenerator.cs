using System.Collections.Generic;
using System.IO;
using System.Linq;using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;

[Generator(LanguageNames.CSharp)]
public sealed class CodeFirstSourceGenerator : IIncrementalGenerator
{    public void Initialize(IncrementalGeneratorInitializationContext context)    {        var allInterfaces = context.CompilationProvider.Select(            (compilation, cancel) => compilation.SyntaxTrees.Select(                tree => compilation.GetSemanticModel(tree)            ).SelectMany(                semanticModel => semanticModel.SyntaxTree                    .GetRoot()                    .DescendantNodes()                    .OfType<InterfaceDeclarationSyntax>()                    .Select(                        declaration => semanticModel.GetDeclaredSymbol(declaration)                    )            )        );        var markedInterfaces = allInterfaces.SelectMany(            (interfaces, cancel) => interfaces.Where(                i => i?.GetAttributes()                    .Any(x => x.AttributeClass?.Name == "CodeFirstTypealized") ?? false            )        );

        context.RegisterSourceOutput(markedInterfaces,            (ctxt, source) =>            {            });    }
}
