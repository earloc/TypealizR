//HintName: TypealizR_StringFormatter.g.cs
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
namespace Some.Root.Namespace {
    [GeneratedCode("TypealizR.StringFormatterSourceGenerator", "1.0.0.0")]
    internal static partial class TypealizR_StringFormatter
    {
        [DebuggerStepThrough]
        internal static LocalizedString Format(this LocalizedString that, params object[] args) => new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);
        internal static LocalizedString Or(this LocalizedString that, LocalizedString fallBack) => that.ResourceNotFound ? fallBack : that;
        internal static partial string Format(string s, object[] args);
        internal static partial string Extend(this string argument, string extension);
        internal static partial int Extend(this int argument, string extension);
        internal static partial uint Extend(this uint argument, string extension);
        internal static partial long Extend(this long argument, string extension);
        internal static partial ulong Extend(this ulong argument, string extension);
        internal static partial short Extend(this short argument, string extension);
        internal static partial ushort Extend(this ushort argument, string extension);
        internal static partial single Extend(this single argument, string extension);
        internal static partial float Extend(this float argument, string extension);
        internal static partial double Extend(this double argument, string extension);
        internal static partial decimal Extend(this decimal argument, string extension);
        internal static partial bool Extend(this bool argument, string extension);
        internal static partial DateTime Extend(this DateTime argument, string extension);
        internal static partial DateTimeOffset Extend(this DateTimeOffset argument, string extension);
        internal static partial Guid Extend(this Guid argument, string extension);
        internal static partial Uri Extend(this Uri argument, string extension);
        internal static partial DateOnly Extend(this DateOnly argument, string extension);
        internal static partial TimeOnly Extend(this TimeOnly argument, string extension);
    }
    internal static partial class TypealizR_StringFormatter
    {
        [DebuggerStepThrough]
        internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
        internal static partial string Extend(this string argument, string extension) => argument;
        internal static partial int Extend(this int argument, string extension) => argument;
        internal static partial uint Extend(this uint argument, string extension) => argument;
        internal static partial long Extend(this long argument, string extension) => argument;
        internal static partial ulong Extend(this ulong argument, string extension) => argument;
        internal static partial short Extend(this short argument, string extension) => argument;
        internal static partial ushort Extend(this ushort argument, string extension) => argument;
        internal static partial single Extend(this single argument, string extension) => argument;
        internal static partial float Extend(this float argument, string extension) => argument;
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
}