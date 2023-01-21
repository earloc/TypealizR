using System;
using System.Linq;

namespace TypealizR;

internal class CodeFirstMethodModel
{
    private readonly string name;
    private readonly string resourceKey;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel (string name, CodeFirstParameterModel[] parameters, string returnType, string? defaultValue)
    {
        this.name = name;
        this.parameters = parameters;
        this.returnType = returnType;
        this.resourceKey = defaultValue ?? $"{name} {parameters.Select((x, i) => $$"""{{{i}}}""").ToSpaceDelimited()}";
    }

    internal string ToCSharp() => $$"""
        public {{returnType}} {{name}} ({{parameters.ToCharpDeclaration()}}) => localizer["{{resourceKey}}", {{parameters.ToCSharpInvocation()}}];
        """;
}
