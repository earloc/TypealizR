using System;
using System.Collections.Generic;
using System.Text;

namespace TypealizR.SourceGenerators.Extensibility;
internal class StringFormatterClassBuilder
{
	private string rootNamespace;

	public StringFormatterClassBuilder(string rootNamespace)
	{
		this.rootNamespace = rootNamespace;
	}

	private bool isUserModeImplementationProvided = false;
	internal void UserModeImplementationIsProvided() => isUserModeImplementationProvided = true;

	internal string Build()
	{
		var stringFormatterStub = $@"
namespace global::Some.Name.Space {{
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args);
	}}";

		var defaultImplementation = default(string?);
		if (isUserModeImplementationProvided)
		{
			defaultImplementation = $@"
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args) => string.Format(global::System.Globalization.CultureInfo.CurrentCulture, s, args)
	}}";
		}

		var builder = new StringBuilder();
		builder.AppendLine(stringFormatterStub);
		if (!string.IsNullOrEmpty(defaultImplementation))
		{
			builder.AppendLine(defaultImplementation);
		}
		builder.AppendLine("}");

		return builder.ToString();
	}
}
