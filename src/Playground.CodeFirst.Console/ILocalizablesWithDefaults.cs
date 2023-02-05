using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;

[CodeFirstTypealized]
public interface ILocalizablesWithDefaults
{
    ///<summary>
    ///42
    ///</summary>
    LocalizedString WhatIsTheMeaningOfLifeTheUniverseAndEverything { get; }

    /// <summary>
    /// Greetings to you, {0}
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    LocalizedString Hello(string world);

    ///<summary>
    ///Goodbye, <paramref name="user"/>
    ///</summary>
    ///<param name="user"></param>
    ///<returns></returns>
    LocalizedString Farewell(string user);

    /// <summary>
    /// <paramref name="right"/> greets <paramref name="left"/>, and <paramref name="left"/> answers: "Hi!".
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    LocalizedString Greet(string left, string right);


}