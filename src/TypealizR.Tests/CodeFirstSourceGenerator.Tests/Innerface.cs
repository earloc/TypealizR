using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

internal partial class Some {
    internal partial class Outer {
        internal partial class NestedClass {
            [CodeFirstTypealized]
            internal interface ITranslations
            {
                LocalizedString Hello(string world);
                LocalizedString World { get; }
            }
        }
    }
}