using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IInterfaceWithMarker
{
    bool IsEnabledProperty { get; }
    bool IsEnabledMethod();

    LocalizedString Greeting { get; }
    LocalizedString Hello(string world);
}

internal interface IInterfaceWithoutMarker
{
}

internal class SomeClass
{
}

internal class SomeOtherClass
{
    internal class SomeNestedClass
    {
        [CodeFirstTypealized]
        internal interface IInterfaceWithMarker
        {
        }
        internal interface IInterfaceWithoutMarker
        {
        }
    }
}