//HintName: TypealizR_StringFormatter.g.cs
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
namespace Some.Root.Namespace {

	[GeneratedCode("TypealizR.SourceGenerator", "1.0.0.0")]
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{
		internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
			new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

		internal static partial string Format(string s, object[] args);
	}

	[GeneratedCode("TypealizR.SourceGenerator", "1.0.0.0")]
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter {
		internal static partial string Format(string s, object[] args) => 
			string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
	}
}

