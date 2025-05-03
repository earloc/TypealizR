using Microsoft.Extensions.Localization;

namespace Playground;

public partial class Discoverer
{
    [TypealizR.EnumerateLocalizers]
    internal partial IStringLocalizer[] GetAll(IServiceProvider sp);
}