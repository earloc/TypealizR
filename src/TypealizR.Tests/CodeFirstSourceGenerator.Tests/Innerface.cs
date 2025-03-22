using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

internal class SomeOuterClass {

    [CodeFirstTypealized]
    internal interface II18n
    {
        LocalizedString Greeting { get; }

        LocalizedString Hello(string world);

        LocalizedString Hello(string user, string world, int visitCount, bool dontPanic);
    }
}