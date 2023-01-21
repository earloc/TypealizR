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

        return new CodeFirstClassModel($"{type.FullName}.g.cs", typealizedInterface, type, methodModels);
    }

    internal CodeFirstMethodBuilder WithMethod(string name)
    {
        var builder = new CodeFirstMethodBuilder(name);

        methodBuilders.Add(builder);

        return builder;
    }
}
