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
		var stringFormatterStub = "a";

		var defaultImplementation = default(string?);
		if (isUserModeImplementationProvided)
		{
			defaultImplementation = "b";
		}

		var builder = new StringBuilder();
		builder.AppendLine(stringFormatterStub);
		if (!string.IsNullOrEmpty(defaultImplementation))
		{
			builder.AppendLine(defaultImplementation);
		}

		return builder.ToString();
	}
}
