using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypealizR;

internal enum Visibility
{
    Internal,
    Public
}

internal static class AccessibilityExtensions {

    public static Visibility ToVisibilty(this Accessibility that) => that switch
    {
        Accessibility.Public => Visibility.Public,
        _ => Visibility.Internal
    };

}
