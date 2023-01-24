using System;
using System.Xml.Linq;

namespace TypealizR;

internal class CodeFirstPropertyModel
{
    private readonly string name;
    private readonly string returnType;
    private readonly string resourceKey;

    public CodeFirstPropertyModel(string name, string returnType, string resourceKey)
    {
        this.name = name;
        this.returnType = returnType;
        this.resourceKey = resourceKey;
    }

    internal string ToCSharp() => $$"""
        public {{returnType}} {{name}} => localizer[@"{{resourceKey}}"];
        """;
}
