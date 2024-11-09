using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.CodeAnalysis;
internal static class SymbolInfoExtensions
{
    const string wantedNameSpace = "Microsoft.Extensions.Localization";
    const string wantedTypeName = "IStringLocalizer";
    const string wantedTypeFullName = $"{wantedNameSpace}.{wantedTypeName}";

    private static INamedTypeSymbol? TryGetTypeOf(ISymbol symbol)
    {

        var symbolType = symbol switch
        {
            ILocalSymbol x => x.Type,
            IMethodSymbol x => x.ReturnType,
            IPropertySymbol x => x.Type,
            IParameterSymbol x => x.Type,
            _ => null
        };


        if (symbolType is not INamedTypeSymbol namedSymbolType)
        {
            return default;
        }

        return namedSymbolType;

    }

    internal static string? EnsureStringLocalizerSymbolName(this SymbolInfo that)
    {
        if (that.Symbol is null)
        {
            return default;
        }

        var displayName = TryGetTypeOf(that.Symbol)?.ToDisplayString();

        if (displayName is null)
        {
            return null;
        }

        var nonGenericDisplayName = displayName.Trim('?').Split('<')[0];

        return nonGenericDisplayName != wantedTypeFullName ? null : that.Symbol.Name;
    }

    internal static ITypeSymbol? TryGetTypeParamForStringLocalizer(this SymbolInfo that)
    {
        if (that.Symbol is null)
        {
            return default;
        }

        var symbolType = TryGetTypeOf(that.Symbol);

        if (symbolType is null)
        {
            return default;
        }

        return symbolType.TypeArguments.FirstOrDefault();


    }
}
