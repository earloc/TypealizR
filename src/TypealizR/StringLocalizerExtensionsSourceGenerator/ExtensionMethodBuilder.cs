using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis;
using System.Xml.Linq;
using TypealizR.Extensions;
using TypealizR.Diagnostics;
using TypealizR.Core;namespace TypealizR;
internal class ExtensionMethodBuilder
{
	private readonly TypeModel markerType;
	private readonly string key;
	private readonly string value;
	private readonly DiagnosticsCollector diagnostics;
	private readonly ParameterBuilder parameterBuilder;

	public ExtensionMethodBuilder(TypeModel markerType, string key, string value, DiagnosticsCollector diagnostics)
	{
		this.markerType = markerType;
		this.key = key;
		this.value = value;
		this.diagnostics = diagnostics;
		parameterBuilder = new(key);
	}

	public ExtensionMethodModel Build()
	{
		var parameters = parameterBuilder.Build(diagnostics);

		var methodNameWithoutParameters = key;

		foreach (var parameter in parameters)
		{
			methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, $"_{parameter.Name}_");
		}

		var name = new MemberName(methodNameWithoutParameters.Trim());

		return new ExtensionMethodModel(markerType, key, value, name, parameters);
	}
}
