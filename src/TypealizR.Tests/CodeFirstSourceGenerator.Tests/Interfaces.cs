using TypealizR.CodeFirst.Abstractions;

namespace TypealizR.Tests.CodeFirst;

[CodeFirstTypealized]
internal interface IInterfaceWithMarker
{
}

internal interface IInterfaceWithoutMarker
{
}

internal class SomeClass
{
}

internal class SomeOtherClass
{
    internal class SomeNestedClass
    {
        [CodeFirstTypealized]
        internal interface IInterfaceWithMarker
        {
        }
        internal interface IInterfaceWithoutMarker
        {
        }
    }
}