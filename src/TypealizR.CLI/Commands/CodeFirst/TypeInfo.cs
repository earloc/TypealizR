using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR.CLI.Commands.CodeFirst;

internal class TypeInfo
{
    public ClassDeclarationSyntax Declaration { get; }
    public InterfaceInfo ImplementingInterface { get; }

    public TypeInfo(ClassDeclarationSyntax declaration, InterfaceInfo implementingInterface)
    {
        Declaration = declaration;
        ImplementingInterface = implementingInterface;
    }
}
