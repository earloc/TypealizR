using System;

using System.Collections.Generic;
using System.Text;

namespace TypealizR.SourceGenerators.Extensibility;
internal class StringFormatterClassBuilder
{
	public static string TypeName = "TypealizR_StringFormatter";

	private readonly string rootNamespace;

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

		builder.Append(GenerateUsings());
		builder.AppendLine(OpenNamespace(this.rootNamespace));
		builder.AppendLine(stringFormatterStub);

		if (!string.IsNullOrEmpty(defaultImplementation))
		{
			builder.AppendLine(defaultImplementation);
		}

		builder.AppendLine(CloseNamespace());
		return builder.ToString();
	}

	private string GenerateUsings() => $@"
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;
";

	private string OpenNamespace(string rootNamespace) =>$@"namespace {rootNamespace} {{";

	private string GenerateStub() => $@"
	internal static partial class {TypeName}
	{{
		internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
			new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

		internal static partial string Format(string s, object[] args);
	}}";

	private static string GenerateDefaultImplementation() => $@"
		{_.GeneratedCodeAttribute}
		[DebuggerStepThrough]
		internal static partial class {TypeName} {{
			internal static partial string Format(string s, object[] args) => 
				string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
		}}
";

	private string CloseNamespace() => "}";
}

