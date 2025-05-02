using Microsoft.Extensions.Localization;

namespace FooBar.ConstructorArguments;

public class Foo
{

}

public class Bar
{

}

public class FooBar
{

#pragma warning disable IDE0060 // Remove unused parameter
    public FooBar(IStringLocalizer<Foo> fooCalizer, IStringLocalizer<Bar>? barCalizer)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        
    }
}