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

		string stringFormatterStub = GenerateStub();

		var defaultImplementation = default(string?);
		if (isUserModeImplementationProvided)
		{
			defaultImplementation = GenerateDefaultImplementation();
		}

		var builder = new StringBuilder();
		builder.AppendLine(OpenNamespace(this.rootNamespace));
		builder.AppendLine(stringFormatterStub);

		if (!string.IsNullOrEmpty(defaultImplementation))
		{
			builder.AppendLine(defaultImplementation);
		}

		builder.AppendLine(CloseNamespace());

		return builder.ToString();
	}



	private string OpenNamespace(string rootNamespace) =>$@"namespace global::{rootNamespace} {{";

	private string GenerateStub() => $@"
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args);
	}}";

	private static string GenerateDefaultImplementation() => $@"
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args) => string.Format(global::System.Globalization.CultureInfo.CurrentCulture, s, args)
	}}";

	private string CloseNamespace() => "}";
}

