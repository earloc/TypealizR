using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

internal partial class SomeOuterClass {

    [CodeFirstTypealized]
    internal interface II18n
    {
        LocalizedString Hello(string world);
    }
}