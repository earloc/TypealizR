using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

#pragma warning disable CA1812 // 'Some' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic).
internal sealed partial class Some {
    internal sealed partial class Outer {
        internal sealed partial class NestedClass {
#pragma warning restore CA1812 // 'Some' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic).

            [CodeFirstTypealized]
            internal interface ITranslations
            {
                LocalizedString Hello(string world);
                LocalizedString World { get; }
            }
        }
    }
}