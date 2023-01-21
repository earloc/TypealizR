using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace TypealizR;
internal class CodeFirstClassBuilder
{
    private ImmutableArray<SymbolDisplayPart> targetNamespace;

    public CodeFirstClassBuilder(ImmutableArray<SymbolDisplayPart> targetNamespace)
    {
        this.targetNamespace = targetNamespace;
    }

    internal CodeFirstClassModel Build()
    {
        throw new NotImplementedException();
    }
}
