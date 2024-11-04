using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.CodeAnalysis;
internal static class SymbolInfoExtensions
{
    const string wantedNameSpace = "Microsoft.Extensions.Localization";
    const string wantedTypeName = "IStringLocalizer";
    const string wantedTypeFullName = $"{wantedNameSpace}.{wantedTypeName}";

    internal static string? EnsureStringLocalizerSymbolName(this SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol is null)
        {
            return null;
        }

        var displayName = symbolInfo.Symbol switch
        {
            ILocalSymbol x => x.Type.ToDisplayString(),
            IMethodSymbol x => x.ReturnType.ToDisplayString(),
            IPropertySymbol x => x.Type.ToDisplayString(),
            IParameterSymbol x => x.Type.ToDisplayString(),
            _ => null
        };

        if (displayName is null)
        {
            return null;
        }

        var nonGenericDisplayName = displayName.Trim('?').Split('<')[0];

        return nonGenericDisplayName != wantedTypeFullName ? null : symbolInfo.Symbol.Name;
    }
}
