
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypealizR.Core;

namespace TypealizR.Extensions;

public static class MicrosoftCodeAnalysisExtensions
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

    public static Accessibility InferrAccessibility(this SyntaxTokenList modifiers)
    {
        if (modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
        {
            return Accessibility.Public;
        }
        if (modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
        {
            return Accessibility.Internal;
        }
        if (modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
        {
            return Accessibility.Private;
        }
        if (modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
        {
            return Accessibility.Protected;
        }
        return Accessibility.Internal;
    }

    public static Visibility ToVisibilty(this Accessibility that) => that switch
    {
        Accessibility.Public => Visibility.Public,
        _ => Visibility.Internal
    };
}