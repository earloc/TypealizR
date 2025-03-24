using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace Playground.CodeFirst.Console;
#pragma warning disable CA1812 // 'Some.Inner' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static'
internal sealed partial class Some {
    internal sealed partial class Inner {
#pragma warning restore CA1812 // 'Some.Inner' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static'

        [CodeFirstTypealized]
        internal interface ISampleInnerface {
            LocalizedString Hello { get; }
            /// <summary>
            /// Hello '<paramref name="world"/>'
            LocalizedString World(string world);
        }
    }
}