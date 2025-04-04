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
        internal static global::System.String Extend(global::System.String argument, string extension);
        internal static global::System.Int32 Extend(global::System.Int32 argument, string extension);
        internal static global::System.Int64 Extend(global::System.Int64 argument, string extension);
        internal static global::System.Single Extend(global::System.Single argument, string extension);
        internal static global::System.Double Extend(global::System.Double argument, string extension);
        internal static global::System.Decimal Extend(global::System.Decimal argument, string extension);
        internal static global::System.Boolean Extend(global::System.Boolean argument, string extension);
        internal static global::System.DateTime Extend(global::System.DateTime argument, string extension);
        internal static global::System.DateTimeOffset Extend(global::System.DateTimeOffset argument, string extension);
        internal static global::System.Guid Extend(global::System.Guid argument, string extension);
        internal static global::System.Uri Extend(global::System.Uri argument, string extension);
    }
    internal static partial class TypealizR_StringFormatter
    {
        [DebuggerStepThrough]
        internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
        internal static global::System.String Extend(global::System.String argument, string extension) => argument;
        internal static global::System.Int32 Extend(global::System.Int32 argument, string extension) => argument;
        internal static global::System.Int64 Extend(global::System.Int64 argument, string extension) => argument;
        internal static global::System.Single Extend(global::System.Single argument, string extension) => argument;
        internal static global::System.Double Extend(global::System.Double argument, string extension) => argument;
        internal static global::System.Decimal Extend(global::System.Decimal argument, string extension) => argument;
        internal static global::System.Boolean Extend(global::System.Boolean argument, string extension) => argument;
        internal static global::System.DateTime Extend(global::System.DateTime argument, string extension) => argument;
        internal static global::System.DateTimeOffset Extend(global::System.DateTimeOffset argument, string extension) => argument;
        internal static global::System.Guid Extend(global::System.Guid argument, string extension) => argument;
        internal static global::System.Uri Extend(global::System.Uri argument, string extension) => argument;
    }
}