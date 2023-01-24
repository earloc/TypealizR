// See https://aka.ms/new-console-template for more information
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.CodeFirst.Console;

[CodeFirstTypealized]
public interface ILocalizables
{
    ///<summary>
    ///Hello, <param name="world"/>
    ///</summary>
    ///<param name="world"></param>
    ///<returns></returns>
    LocalizedString Hello(string world);

    ///<summary>
    ///Goodbye, <paramref name="user"/>
    ///</summary>
    ///<param name="user"></param>
    ///<returns></returns>
    LocalizedString Farewell(string user);

    ///<summary>
    ///42
    ///</summary>
    LocalizedString WhatIsTheMeaningOfLifeTheUniverseAndEverything { get; }
}
