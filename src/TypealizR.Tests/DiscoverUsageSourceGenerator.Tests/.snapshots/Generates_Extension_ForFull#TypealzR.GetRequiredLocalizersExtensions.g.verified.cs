//HintName: TypealzR.GetRequiredLocalizersExtensions.g.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypealzR_GetRequiredLocalizersExtensions
    {
        public static IStringLocalizer[] GetRequiredLocalizers(this IServiceProvider sp)
        {
            return 
            [
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<FooBar.Full.Foo>>(),
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<FooBar.Full.Bar>>(),
            ]
        }
    }
}