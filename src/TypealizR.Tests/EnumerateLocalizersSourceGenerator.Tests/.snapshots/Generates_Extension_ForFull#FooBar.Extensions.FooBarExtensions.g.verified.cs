//HintName: FooBar.Extensions.FooBarExtensions.g.cs
namespace FooBar.Extensions;

internal partial class FooBarExtensions
{
    [EnumerateLocalizers]
    internal static partial IEnumerable<IStringLocalizer> GetAll(IServiceProvider sp)
    {
        yield return sp.GetRquiredService<IStringLocalizert<Foo>();
        yield return sp.GetRquiredService<IStringLocalizert<Bar>();
    }
}