using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;

[CodeFirstTypealized]
internal interface ILocalizables
{
    LocalizedString Hello(string world);

    ///<summary>
    /// 42
    ///</summary>
    LocalizedString WhatIsTheMeaningOfLifeTheUniverseAndEverything { get; }

    ///<summary>
    /// Goodbye, <paramref name="user"/>
    ///</summary>
    LocalizedString Farewell(string user);

    /// <summary>
    /// <paramref name="right"/> greets <paramref name="left"/>, and <paramref name="left"/> answers: "Hi!".
    /// </summary>
    LocalizedString Greet(string left, string right);

    /// <summary>
    /// Goodbye, <paramref name="name"/> and thx for all the fish!
    /// </summary>
    LocalizedString Goodbye(string name);
}