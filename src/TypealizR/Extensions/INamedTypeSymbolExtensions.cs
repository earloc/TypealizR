
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.CodeAnalysis;

public static class INamedTypeSymbolExtensions
{
    public static string[] GetContainingTypesRecursive(this INamedTypeSymbol? type, IList<string>? containingTypeNames = null)
    {
        if (containingTypeNames is null)
        {
            containingTypeNames = new List<string>();
        }

        if (type is null)
        {
            return containingTypeNames.AsEnumerable().Reverse().ToArray();
        }

        containingTypeNames.Add(type.Name);

        return type.ContainingType.GetContainingTypesRecursive(containingTypeNames);
    }
}