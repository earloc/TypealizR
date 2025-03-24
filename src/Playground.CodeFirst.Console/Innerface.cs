using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;

internal sealed partial class Some {
    internal sealed partial class Inner {
        [CodeFirstTypealized]
        internal interface ISampleInnerface {
            LocalizedString Hello { get; }
            /// <summary>
            /// Hello '<paramref name="world"/>'
            LocalizedString World(string world);
        }
    }
}