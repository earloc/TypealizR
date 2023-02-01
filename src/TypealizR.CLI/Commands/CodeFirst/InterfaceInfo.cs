using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR.CLI.Commands.CodeFirst;

internal class InterfaceInfo
{
    public InterfaceDeclarationSyntax Declaration { get; }
    public ISymbol Symbol { get; }

    public InterfaceInfo(InterfaceDeclarationSyntax declaration, ISymbol symbol)
    {
        Declaration = declaration;
        Symbol = symbol;
    }
}
