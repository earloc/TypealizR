using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;

[CodeFirstTypealized]
internal interface ILocalizablesWithDefaults
{
    ///<summary>
    /// 42
    ///</summary>
    ///<remarks>
    /// The famous answer to the lesser known question...
    /// </remarks>
    LocalizedString WhatIsTheMeaningOfLifeTheUniverseAndEverything { get; }

    /// <summary>
    /// Greetings to you, {0}
    /// </summary>
    ///<remarks>
    /// What someone says to someone else when they meet
    /// </remarks>
    LocalizedString Hello(string world);

    ///<summary>
    /// Goodbye, <paramref name="user"/>
    ///</summary>
    ///<remarks>
    /// What someone says to someone else when they depart
    /// </remarks>
    LocalizedString Farewell(string user);

    /// <summary>
    /// <paramref name="right"/> greets <paramref name="left"/>, and <paramref name="left"/> answers: 'Hi!'.
    /// </summary>
    /// <remarks>
    /// A sample conversation
    /// </remarks>
    LocalizedString Greet(string left, string right);


}