using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface ITranslatables
{
    bool IsEnabledProperty { get; }
    bool IsEnabledMethod();

    LocalizedString Greeting { get; }

    /// <summary>
    /// Hello, fellow developer!
    /// </summary>
    LocalizedString CustomizedGreeting { get; }

    LocalizedString Hello(string world);

    /// <summary>
    /// Hello {0}!
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    LocalizedString CustomizedHello(string world);

    LocalizedString Hello(string user, string world, int visitCount, bool dontPanic);

    /// <summary>
    /// Hello <paramref name="user"/>, the current time is: <paramref name="now"/>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    LocalizedString Greet(string user, DateTimeOffset now);

    /// <summary>
    /// The current time is: <paramref name="now"/>, Goodbye <paramref name="user"/>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    LocalizedString Farewell(string user, DateTimeOffset now);

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