﻿using System;
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
    private readonly List<CodeFirstParameterBuilder> parameterBuilders = new();

    public CodeFirstMethodBuilder(string name)
    {
        this.name = name;
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

        var method = new CodeFirstMethodModel(name, parameters, "LocalizedString");

        return method;
    }

    
}