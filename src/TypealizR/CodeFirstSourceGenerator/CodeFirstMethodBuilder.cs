using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR;
internal class CodeFirstMethodBuilder
{
    private readonly string name;
    private readonly string? defaultValue;
    private readonly List<CodeFirstParameterBuilder> parameterBuilders = new();

    public CodeFirstMethodBuilder(string name, string? defaultValue)
    {
        this.name = name;
        this.defaultValue = defaultValue;
    }

    public CodeFirstMethodBuilder WithParameter(string name, string type)
    {
        var builder = new CodeFirstParameterBuilder(name, type);
        parameterBuilders.Add(builder);
        return this;
    }

    internal CodeFirstMethodModel Build()
    {
        var parameters = parameterBuilders
            .Select(x => x.Build())
            .ToArray()
        ;

        var customDefaultValue = defaultValue ?? name;

        if (defaultValue is not null)
        {
            var indexedParams = parameters.Select((x, i) => new { Name = x.Name, Index = i }).ToArray();

            foreach (var param in indexedParams)
            {
                customDefaultValue = customDefaultValue.Replace($$"""{{{param.Name}}}""", $$"""{{{param.Index}}}""").Trim();
            }
        }

        var method = new CodeFirstMethodModel(name, parameters, "LocalizedString", customDefaultValue);

        return method;
    }

    
}
