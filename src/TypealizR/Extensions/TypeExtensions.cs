using System;

namespace TypealizR.Extensions;
internal static class TypeExtensions
{
    internal static string GeneratedCodeAttribute(this Type that) => $@"[GeneratedCode(""{that.FullName}"", ""{that.Assembly.GetName().Version}"")]";
}
