using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;

internal partial class Some {
    internal partial class Inner {
        [CodeFirstTypealized]
        internal interface ITranslations {
            LocalizedString Hello { get; }
            /// <summary>
            /// Hello '<paramref name="world"/>'
            LocalizedString World(string world);
        }
    }
}