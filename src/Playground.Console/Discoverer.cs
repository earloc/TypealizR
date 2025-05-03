using Microsoft.Extensions.Localization;

namespace Playground;

public partial class Discoverer
{
    [TypealizR.EnumerateLocalizers]
    internal partial IEnumerable<IStringLocalizer> GetAll(IServiceProvider sp);
}