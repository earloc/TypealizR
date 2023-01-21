using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface ITranslatables
{
    bool IsEnabledProperty { get; }
    bool IsEnabledMethod();

    LocalizedString Greeting { get; }

    LocalizedString Hello(string world);

    LocalizedString Hello(string user, string world, int visitCount, bool dontPanic);
}

internal interface IInterfaceWithoutMarker
{
}

internal class SomeClass
{
}

//internal class SomeOtherClass
//{
//    internal class SomeNestedClass
//    {
//        [CodeFirstTypealized]
//        internal interface ITranslatables
//        {
//        }
//        internal interface IInterfaceWithoutMarker
//        {
//        }
//    }
//}