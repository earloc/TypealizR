using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR.CLI.Commands.CodeFirst;

internal class PropertyInfo
{
    public PropertyDeclarationSyntax Syntax { get; }
    public IdentifierNameSyntax ReturnType { get; }

    public PropertyInfo(PropertyDeclarationSyntax syntax, IdentifierNameSyntax returnType)
    {
        Syntax = syntax;
        ReturnType = returnType;
    }
}
