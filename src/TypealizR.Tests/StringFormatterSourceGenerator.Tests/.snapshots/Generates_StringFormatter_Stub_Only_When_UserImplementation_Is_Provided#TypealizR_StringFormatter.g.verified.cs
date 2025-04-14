//HintName: TypealizR_StringFormatter.g.cs
using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
namespace Some.Root.Namespace {
    [GeneratedCode("TypealizR.StringFormatterSourceGenerator", "1.0.0.0")]
    internal static partial class TypealizR_StringFormatter
    {
        internal static LocalizedString Format(LocalizedString that, params object[] args) => new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);
        internal static partial string Format(string s, object[] args);
        internal static partial string Extend(string argument, string extension);
        internal static partial int Extend(int argument, string extension);
        internal static partial uint Extend(uint argument, string extension);
        internal static partial long Extend(long argument, string extension);
        internal static partial ulong Extend(ulong argument, string extension);
        internal static partial short Extend(short argument, string extension);
        internal static partial ushort Extend(ushort argument, string extension);
        internal static partial float Extend(float argument, string extension);
        internal static partial double Extend(double argument, string extension);
        internal static partial decimal Extend(decimal argument, string extension);
        internal static partial bool Extend(bool argument, string extension);
        internal static partial DateTime Extend(DateTime argument, string extension);
        internal static partial DateTimeOffset Extend(DateTimeOffset argument, string extension);
        internal static partial Guid Extend(Guid argument, string extension);
        internal static partial Uri Extend(Uri argument, string extension);
        internal static partial DateOnly Extend(DateOnly argument, string extension);
        internal static partial TimeOnly Extend(TimeOnly argument, string extension);
    }
    internal static partial class TypealizR_StringFormatter
    {
    }
}