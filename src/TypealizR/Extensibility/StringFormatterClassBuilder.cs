using System;

using System.Collections.Generic;
using System.Text;

namespace TypealizR.Extensibility;
internal class StringFormatterClassBuilder
{
	internal static readonly string TypeName = "TypealizR_StringFormatter";

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

		builder.AppendLine();
		builder.AppendLine(stringFormatterStub);

		if (!string.IsNullOrEmpty(defaultImplementation))
		{
			builder.AppendLine();
			builder.AppendLine(defaultImplementation);
		}

		builder.AppendLine(CloseNamespace);
		builder.AppendLine();

		return builder.ToString();
	}

	private string GenerateUsings() => $"""
		using System.Diagnostics;
		using System.CodeDom.Compiler;
		using Microsoft.Extensions.Localization;

		""";

	private string OpenNamespace(string rootNamespace) =>$@"namespace {rootNamespace} {{";

	private string GenerateStub() => $$"""
		{{_.GeneratedCodeAttribute}}
		internal static partial class {{TypeName}}
		{
			[DebuggerStepThrough]
			internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
				new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

			internal static partial string Format(string s, object[] args);
		}
	""";

	private static string GenerateDefaultImplementation() => $$"""
		internal static partial class {{TypeName}} {
			[DebuggerStepThrough]
			internal static partial string Format(string s, object[] args) => 
				string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
		}
	""";

	private const string CloseNamespace = "}";
}

