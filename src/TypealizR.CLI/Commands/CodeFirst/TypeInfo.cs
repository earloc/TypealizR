using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR.CLI.Commands.CodeFirst;

internal class TypeInfo
{
    public ClassDeclarationSyntax Declaration { get; }
    public SemanticModel Model { get; }
    public INamedTypeSymbol Symbol { get; }
    public InterfaceInfo ImplementingInterface { get; }

    public TypeInfo(ClassDeclarationSyntax declaration, SemanticModel model, INamedTypeSymbol symbol, InterfaceInfo implementingInterface)
    {
        Declaration = declaration;
        Model = model;
        Symbol = symbol;
        ImplementingInterface = implementingInterface;
    }
}
