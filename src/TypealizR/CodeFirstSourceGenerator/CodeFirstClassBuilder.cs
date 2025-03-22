using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;

namespace TypealizR;
internal class CodeFirstClassBuilder(TypeModel typealizedInterface)
{
    internal readonly List<CodeFirstMethodBuilder> methodBuilders = [];
    internal readonly List<CodeFirstPropertyBuilder> propertyBuilders = [];

    private readonly TypeModel typealizedInterface = typealizedInterface;
    private readonly TypeModel type = new TypeModel(typealizedInterface.Namespace, typealizedInterface.Name.Trim('I'));

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

        return new ($"{typealizedInterface.FullName}.g.cs", typealizedInterface, type, methodModels, propertyModels);
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
