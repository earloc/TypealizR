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

namespace TypealizR.Builder;
internal class InstanceMemberBuilder : IMemberBuilder
{
	private readonly string key;
	private readonly string value;
	private readonly ParameterBuilder parameterBuilder;

	public InstanceMemberBuilder(string key, string value)
	{
		this.key = key;
		this.value = value;
		parameterBuilder = new(key);
	}

	public IMemberModel Build(TypeModel target, DiagnosticsCollector diagnostics)
	{
		var parameters = parameterBuilder.Build(diagnostics);

		var methodNameWithoutParameters = key;

		foreach (var parameter in parameters)
		{
			methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, $"_{parameter.Name}_");
		}

		string compilableMethodName = SanitizeMethodName(methodNameWithoutParameters.Trim());

		return new InstanceMemberModel(target, key, value, compilableMethodName, parameters);
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