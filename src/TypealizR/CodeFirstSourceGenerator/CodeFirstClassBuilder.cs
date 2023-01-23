using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Core;

namespace TypealizR;
internal class CodeFirstClassBuilder
{
    internal readonly List<CodeFirstMethodBuilder> methodBuilders = new();
    internal readonly List<CodeFirstPropertyBuilder> propertyBuilders = new();

    private readonly TypeModel typealizedInterface;
    private readonly TypeModel type;

    public CodeFirstClassBuilder(TypeModel typealizedInterface)
    {
        this.typealizedInterface = typealizedInterface;
        type = new TypeModel(typealizedInterface.Namespace, typealizedInterface.Name.Trim('I'));
    }

    internal CodeFirstClassModel Build()
    {
        var methodModels = methodBuilders
            .Select(x => x.Build())
            .ToArray()
        ;

        var propertyModels = propertyBuilders
            .Select(x => x.Build())
            .ToArray()
        ;

        return new CodeFirstClassModel($"{typealizedInterface.FullName}.g.cs", typealizedInterface, type, methodModels, propertyModels);
    }

    internal CodeFirstMethodBuilder WithMethod(string name, string? defaultValue)
    {
        var builder = new CodeFirstMethodBuilder(name, defaultValue);
        methodBuilders.Add(builder);
        return builder;
    }

    internal CodeFirstPropertyBuilder WithProperty(string name, string? defaultValue)
    {
        var builder = new CodeFirstPropertyBuilder(name, defaultValue);
        propertyBuilders.Add(builder);
        return builder;
    }
}
