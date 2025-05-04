using Microsoft.CodeAnalysis;

namespace TypealizR.Core;

internal enum Visibility
{
    Internal,
    Public
}

internal static class AccessibilityExtensions
{

    public static Visibility ToVisibilty(this Accessibility that) => that switch
    {
        Accessibility.Public => Visibility.Public,
        _ => Visibility.Internal
    };

    public static string ToCSharp(this Accessibility that) => that switch
    {
        Accessibility.Private => "private",
        Accessibility.ProtectedAndInternal => "protected internal",
        Accessibility.Protected => "protected",
        Accessibility.Internal => "internal",
        Accessibility.Public => "public",
        _ => ""
    };
}
