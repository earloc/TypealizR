using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.BlazorServer9.Components;

[CodeFirstTypealized]
public interface IStrings
{
    /// <summary>
    /// Hallo
    /// </summary>
    LocalizedString Title { get; }

    /// <summary>
    /// Welt
    /// </summary>
    LocalizedString Title2 { get; }

}
