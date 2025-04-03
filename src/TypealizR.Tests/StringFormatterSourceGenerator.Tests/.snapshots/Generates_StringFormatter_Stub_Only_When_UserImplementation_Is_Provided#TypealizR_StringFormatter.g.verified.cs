//HintName: TypealizR_StringFormatter.g.cs
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
namespace Some.Root.Namespace {
    [GeneratedCode("TypealizR.StringFormatterSourceGenerator", "1.0.0.0")]
    internal static partial class TypealizR_StringFormatter
    {
            [DebuggerStepThrough]
            internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
                new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);
            [DebuggerStepThrough]
            internal static object Extend(this object that, string extension) => ExtendArg(that, extension);
            internal static LocalizedString Or(this LocalizedString that, LocalizedString fallBack) => 
                that.ResourceNotFound ? fallBack : that;
        internal static partial string Format(string s, object[] args);
        internal static partial object ExtendArg(object arg, string annotationExtension);
    }
}