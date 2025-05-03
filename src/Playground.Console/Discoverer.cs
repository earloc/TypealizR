using Microsoft.Extensions.Localization;

namespace FooBar.Extensions;

public partial class FooBarExtensions
{
    [TypealizR.EnumerateLocalizers]
    internal partial IStringLocalizer[] GetAll(IServiceProvider sp);
}