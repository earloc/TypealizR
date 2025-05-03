using Microsoft.Extensions.Localization;

namespace FooBar.Properties;

public sealed class Foo
{

}

public sealed class Bar
{

}

public sealed class FooBarClass
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IStringLocalizer<Foo> FooCalizer { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IStringLocalizer<Bar>? BarCalizer { get; }
}