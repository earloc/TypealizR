//HintName: TypealzR.GetRequiredLocalizersExtensions.g.cs
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypealzR_GetRequiredLocalizersExtensions
    {
        public static global::Microsoft.Extensions.Localization.IStringLocalizer[] GetRequiredLocalizers(this global::Microsoft.Extensions.DependencyInjection.ServiceProvider sp)
        {
            return 
            [
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<global::FooBar.Full.Foo>>(),
                sp.GetRequiredService<global::Microsoft.Extensions.Localization.IStringLocalizer<global::FooBar.Full.Bar>>()
            ];
        }
    }
}