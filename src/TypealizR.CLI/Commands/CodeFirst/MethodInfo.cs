using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR.CLI.Commands.CodeFirst;

internal class MethodInfo
{
    public MethodDeclarationSyntax Syntax { get; }
    public IdentifierNameSyntax ReturnType { get; }

    public MethodInfo(MethodDeclarationSyntax syntax, IdentifierNameSyntax returnType)
    {
        Syntax = syntax;
        ReturnType = returnType;
    }

    public override bool Equals(object? obj)
    {
        return obj is MethodInfo other &&
               EqualityComparer<MethodDeclarationSyntax>.Default.Equals(Syntax, other.Syntax) &&
               EqualityComparer<IdentifierNameSyntax>.Default.Equals(ReturnType, other.ReturnType);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Syntax, ReturnType);
    }
}