using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI.Abstractions;

namespace TypealizR.CLI.Binders;
internal class FuncBinder<T> : BinderBase<T> where T : notnull
{
    private readonly Func<IServiceProvider, T> configure;

    public FuncBinder(Func<IServiceProvider, T> configure)
	{
        this.configure = configure;
    }

    protected override T GetBoundValue(BindingContext bindingContext)
    {
        bindingContext.AddService(configure);
        return bindingContext.GetRequiredService<T>();
    }
}
