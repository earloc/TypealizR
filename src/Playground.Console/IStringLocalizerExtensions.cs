using Microsoft.Extensions.Localization;

namespace Playground.Console;
internal static class IStringLocalizerExtensions
{
    public static LocalizedString Bar(this IStringLocalizer that) => that["Bar"];
    public static LocalizedString Bar(this IStringLocalizer that, string value) => that["Bar", value];

    public static LocalizedString Foo(this IStringLocalizer that, string value) => that["Foo {bar}", value];
    public static LocalizedString FooBar(this IStringLocalizer that, string value) => that["FooBar {0}", value];

    public static LocalizedString Bars(this IStringLocalizer that, string v1, int v2) => that["Bar {0} {1}", v1, v2];

}