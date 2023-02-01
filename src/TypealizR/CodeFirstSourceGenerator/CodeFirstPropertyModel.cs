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
    private string KeyName => $"{name}_Key";

    internal string ToCSharp() => $$"""
        private const string {{KeyName}} = @"{{resourceKey}}";
                public {{returnType}} {{name}} => localizer[@"{{name}}"].Or(localizer[{{KeyName}}]);
        """;
}
