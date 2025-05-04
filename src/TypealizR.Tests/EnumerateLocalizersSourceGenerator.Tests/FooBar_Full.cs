using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace FooBar.Full;

public sealed class Foo
{

}

public sealed class Bar
{

}

public sealed class FooBarClass
{
    public FooBarClass(IStringLocalizer<Foo> fooCalizer, IStringLocalizer<Bar>? barCalizer)
    {
        FooCalizer = fooCalizer;
        BarCalizer = barCalizer;
    }

    public IStringLocalizer<Foo> FooCalizer { get; }
    public IStringLocalizer<Bar>? BarCalizer { get; }

    public override string ToString()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var loc = provider.GetRequiredService<IStringLocalizer<FooBarClass>>();
        return loc[nameof(FooBarClass)];
    }
}