﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR;
internal class CodeFirstMethodBuilder
{
    private readonly string name;
    private readonly string? defaultValue;
    private readonly List<CodeFirstParameterBuilder> parameterBuilders = [];

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

        var resourceKey = defaultValue;

        if (resourceKey is not null)
        {
            var indexedParams = parameters.Select((x, i) => new { x.Name, Index = i }).ToArray();

            foreach (var param in indexedParams)
            {
                resourceKey = resourceKey.Replace($$"""{{{param.Name}}}""", $$"""{{{param.Index}}}""").Trim();
            }
        }

        var method = new CodeFirstMethodModel(name, parameters, "LocalizedString", resourceKey);

        return method;
    }


}
