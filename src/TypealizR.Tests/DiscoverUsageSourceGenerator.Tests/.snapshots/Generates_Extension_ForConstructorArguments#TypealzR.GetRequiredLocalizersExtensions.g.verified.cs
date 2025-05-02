//HintName: sample.g.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypealizRExtensions
    {
        public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
        {
            return 
            [
                sp.GetRequiredService<IStringLocalizer<global::FooBar.ConstructorArguments.Foo>>(),
                sp.GetRequiredService<IStringLocalizer<global::FooBar.ConstructorArguments.Bar>>()
            ]
        }
    }
}