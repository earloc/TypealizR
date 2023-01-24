using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IMethodsWithXmlCommentParameters
{
    /// <summary>
    /// Hello <paramref name="user"/>, the current time is: <paramref name="now"/>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    LocalizedString Greet(string user, DateTimeOffset now);

    /// <summary>
    /// The current time is: <paramref name="now"/>, goodbye '<paramref name="user"/>'
    /// </summary>
    /// <param name="user"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    LocalizedString Farewell(string user, DateTimeOffset now);

    /// <summary>
    /// 1.<paramref name="name"/> 2.<paramref name="name"/> 3.<paramref name="name"/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    LocalizedString CallForBeetlejuice(string name);

    /// <summary>
    /// <paramref name="verb"/>, <paramref name="name"/>. <paramref name="verb"/>!!
    /// </summary>
    /// <param name="name"></param>
    /// <param name="verb"></param>
    /// <returns></returns>
    LocalizedString DoIt(string name = "Forrest", string verb = "run");
}
