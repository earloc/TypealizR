using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;

namespace TypealizR;

internal class CodeFirstClassBuilder
{
    internal readonly List<CodeFirstMethodBuilder> methodBuilders = [];
    internal readonly List<CodeFirstPropertyBuilder> propertyBuilders = [];
    private readonly TypeModel typealizedInterface;
    private readonly string[] containingTypes;
    private readonly TypeModel implementationType;

    public CodeFirstClassBuilder(TypeModel typealizedInterface, string[] containingTypes)
    {
        this.typealizedInterface = typealizedInterface;
        this.containingTypes = containingTypes;
        var implementationTypeName = typealizedInterface.Name.Trim('I');
        implementationType = new TypeModel(typealizedInterface.Namespace, implementationTypeName, containingTypes);
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

        return new($"{typealizedInterface.FullName}.g.cs", typealizedInterface, implementationType, containingTypes, methodModels, propertyModels);
    }

    internal CodeFirstMethodBuilder WithMethod(string name, string? defaultValue, string? remarks)
    {
        var builder = new CodeFirstMethodBuilder(name, defaultValue, remarks);
        methodBuilders.Add(builder);
        return builder;
    }

    internal CodeFirstPropertyBuilder WithProperty(string name, string? defaultValue, string? remarks)
    {
        var builder = new CodeFirstPropertyBuilder(name, defaultValue, remarks);
        propertyBuilders.Add(builder);
        return builder;
    }
}
