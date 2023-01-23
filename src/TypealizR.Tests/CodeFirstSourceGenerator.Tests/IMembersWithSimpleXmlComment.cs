﻿using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IMembersWithSimpleXmlComment
{
    /// <summary>
    /// Hello {0}!
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    LocalizedString Hello(string world);

    /// <summary>
    /// Hello world!
    /// </summary>
    /// <returns></returns>
    LocalizedString HelloProperty { get; }

    /// <summary>
    /// Greetings, fellow developer!
    /// </summary>
    LocalizedString Greeting { get; }

    /// <summary>
    /// Greetings, fellow developer!
    /// This line here will be in the generated default value, also.
    /// But sadly without the newlines ;(
    /// </summary>
    LocalizedString GreetingWithMultilineComment { get; }

}
