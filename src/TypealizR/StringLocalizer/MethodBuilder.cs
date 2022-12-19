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

namespace TypealizR.StringLocalizer;

internal class MethodBuilder
{
    private readonly string key;
    private readonly string value;
	private readonly ParameterBuilder parameterBuilder;

	public MethodBuilder(string key, string value)
    {
        this.key = key;
        this.value = value;
		parameterBuilder = new (key);
	}

    public MethodModel Build(TypeModel target, DiagnosticsCollector diagnostics)
    {
		var parameters = parameterBuilder.Build(diagnostics);

		var methodNameWithoutParameters = key;

        foreach (var parameter in parameters)
        {
            methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, parameter.Name);
        }

        string compilableMethodName = SanitizeMethodName(methodNameWithoutParameters.Trim());

        return new MethodModel(target, key, value, compilableMethodName, parameters);
    }


    private string SanitizeMethodName(string rawMethodName)
    {
        return new string(
            rawMethodName
                .Replace(" ", "_")
                .Trim('_')
                .Select((x, i) => x.IsValidInIdentifier(i == 0) ? x : ' ')
                .ToArray()
        )
        .Replace(" ", "")
        .Trim('_');
    }
}