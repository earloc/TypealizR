using Microsoft.Extensions.Localization;

namespace TypealizR.Abstractions;

public interface IStringTypealizR<T> : IStringLocalizer<T>
{
}
