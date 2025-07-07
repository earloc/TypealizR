using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace CLI.CodeFirst;

[CodeFirstTypealized]
public interface ILocalizables
{
    /// <summary>
    /// Hello, fellow developer!
    /// </summary>
    public LocalizedString Hello { get; }

    /// <summary>
    /// Hey <paramref name="userName"/>, welcome to <paramref name="planetName"/> 👍!
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="planetName"></param>
    /// <returns></returns>
    public LocalizedString Greet(string userName, string planetName);
}
