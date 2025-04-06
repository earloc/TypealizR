using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Root.Namespace;
internal static partial class TypealizR_StringFormatter
{
    internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
    internal static partial string Extend(this string argument, string extension) => argument;
    internal static partial int Extend(this int argument, string extension) => argument;
    internal static partial uint Extend(this uint argument, string extension) => argument;
    internal static partial float Extend(this float argument, string extension) => argument;
    internal static partial long Extend(this long argument, string extension) => argument;
    internal static partial ulong Extend(this ulong argument, string extension) => argument;
    internal static partial short Extend(this short argument, string extension) => argument;
    internal static partial ushort Extend(this ushort argument, string extension) => argument;
    internal static partial double Extend(this double argument, string extension) => argument;
    internal static partial decimal Extend(this decimal argument, string extension) => argument;
    internal static partial bool Extend(this bool argument, string extension) => argument;
    internal static partial DateTime Extend(this DateTime argument, string extension) => argument;
    internal static partial DateTimeOffset Extend(this DateTimeOffset argument, string extension) => argument;
    internal static partial Guid Extend(this Guid argument, string extension) => argument;
    internal static partial Uri Extend(this Uri argument, string extension) => argument;
    internal static partial DateOnly Extend(this DateOnly argument, string extension) => argument;
    internal static partial TimeOnly Extend(this TimeOnly argument, string extension) => argument;
}
