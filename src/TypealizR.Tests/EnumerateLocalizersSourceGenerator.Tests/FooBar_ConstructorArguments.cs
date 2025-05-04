using Microsoft.Extensions.Localization;

namespace FooBar.ConstructorArguments;

public sealed class Foo
{

}

public sealed class Bar
{

}

public sealed class FooBarClass
{

#pragma warning disable IDE0060 // Remove unused parameter
    public FooBarClass(IStringLocalizer<Foo> fooCalizer, IStringLocalizer<Bar>? barCalizer)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        
    }
}