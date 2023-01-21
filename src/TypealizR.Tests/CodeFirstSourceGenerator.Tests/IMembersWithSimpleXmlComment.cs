using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IMembersWithSimpleXmlComment
{
    /// <summary>
    /// Hello world!
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    LocalizedString HelloWorld { get; }

    /// <summary>
    /// Hello {0}!
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    LocalizedString Hello(string world);

    /// <summary>
    /// Greetings, fellow developer!
    /// </summary>
    LocalizedString Greeting { get; }
}
