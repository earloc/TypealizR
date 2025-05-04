using Microsoft.Extensions.Localization;

namespace FooBar.Extensions;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class EnumerateLocalizersAttribute : Attribute
{
}
