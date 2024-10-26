using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;

namespace Playground.Console.NoCodeGen;
[SuppressMessage(
    "Reliability", "CA1812:'InternalClass' is an internal class that is apparently never instantiated.If so, remove the code from the assembly.If this class is intended to contain only static members, make it 'static' (Module in Visual Basic).",
    Justification = "Markertype needed for resolution of resx-file")
]
internal sealed class InternalClass
{
    public int MyProperty { get; set; }
}
