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
using TypealizR.Core;

namespace TypealizR;
internal class ExtensionMethodBuilder
{
	private readonly TypeModel markerType;
	private readonly string key;
	private readonly string value;
	private readonly DiagnosticsCollector diagnostics;
    private readonly bool useParametersInMethodNames;
    private readonly ParameterBuilder parameterBuilder;

	public ExtensionMethodBuilder(bool useParametersInMethodNames, TypeModel markerType, string key, string value, DiagnosticsCollector diagnostics)
	{
        this.useParametersInMethodNames = useParametersInMethodNames;
		this.markerType = markerType;
		this.key = key;
		this.value = value;
		this.diagnostics = diagnostics;
        parameterBuilder = new(key);
	}

	public ExtensionMethodModel Build()
	{
		var parameters = parameterBuilder.Build(diagnostics);
        string sanitizedMethodName = key;

		foreach (var parameter in parameters)
		{
            var sanitizedParameterName = useParametersInMethodNames ? $"_{parameter.Name}_" : " ";
            sanitizedMethodName = sanitizedMethodName.Replace(parameter.Token, sanitizedParameterName);
		}

		var name = new MemberName(sanitizedMethodName.Trim());

		return new ExtensionMethodModel(markerType, key, value, name, parameters);
	}
}
