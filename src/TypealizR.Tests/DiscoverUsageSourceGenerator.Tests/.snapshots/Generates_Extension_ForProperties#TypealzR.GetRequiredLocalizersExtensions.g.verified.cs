//HintName: sample.g.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypealizRExtensions
    {
        public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
        {
            return 
            [
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<global::FooBar.Properties.Foo>>(),
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<global::FooBar.Properties.Bar>>()
            ]
        }
    }
}