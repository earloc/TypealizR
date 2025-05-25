using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IMembersWithSimpleXmlComment
{
    /// <summary>
    /// Hello {0}!
    /// </summary>
    /// <param name="world"></param>
    /// <remarks>greets someone</remarks>
    /// <returns></returns>
    LocalizedString Hello(string world);

    /// <summary>
    /// Hello world!
    /// </summary>
    /// <returns></returns>
    /// <remarks>greets someone with a property</remarks>
    LocalizedString HelloProperty { get; }

    /// <summary>
    /// Greetings, fellow developer!
    /// </summary>
    /// <remarks>the greeting</remarks>
    LocalizedString Greeting { get; }

    /// <summary>
    /// Greetings, fellow developer!
    /// This line here will be in the generated default resource-key, also.
    /// And also this one, even with newlines #wowh@x0r!
    /// </summary>
    /// <remarks>a multiline greeting</remarks>
    LocalizedString GreetingWithMultilineComment { get; }

}
