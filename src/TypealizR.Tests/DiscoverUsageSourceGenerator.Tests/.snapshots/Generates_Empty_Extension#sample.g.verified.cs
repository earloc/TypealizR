//HintName: sample.g.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypealizRExtensions
    {
        public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
        {
            return [sp.GetRequiredService<IStringLocalizer<FooBarSpace.Foo>>(),sp.GetRequiredService<IStringLocalizer<FooBarSpace.Bar>>(),]
        }
    }
}