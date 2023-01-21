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
    /// The current time is: <paramref name="now"/>, Goodbye '<paramref name="user"/>'
    /// </summary>
    /// <param name="user"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    LocalizedString Farewell(string user, DateTimeOffset now);
}
