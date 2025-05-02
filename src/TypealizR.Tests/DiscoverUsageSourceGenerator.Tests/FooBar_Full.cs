using Microsoft.Extensions.Localization;

namespace FooBar.Full;

public class Foo
{

}

public class Bar
{

}

public class FooBar
{
    public FooBar(IStringLocalizer<Foo> fooCalizer, IStringLocalizer<Bar>? barCalizer)
    {
        FooCalizer = fooCalizer;
        BarCalizer = barCalizer;
    }

    public IStringLocalizer<Foo> FooCalizer { get; }
    public IStringLocalizer<Bar>? BarCalizer { get; }
}