using System;

namespace TypealizR;

internal class CodeFirstMethodModel
{
    private readonly string name;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel (string name, CodeFirstParameterModel[] parameters, string returnType)
    {
        this.name = name;
        this.parameters = parameters;
        this.returnType = returnType;
    }

    internal string ToCSharp() => $$"""
    public {{returnType}} {{name}} ({{parameters.ToCharpDeclaration()}}) => localizer["{{name}}", {{parameters.ToCSharpInvocation()}}];
    """;
}
