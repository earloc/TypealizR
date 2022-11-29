using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TypealizR.Extensions;

namespace TypealizR.StringLocalizer;

internal class MethodBuilder
{
    private readonly string key;
    private readonly string value;
	private readonly DiagnosticsFactory diagnostics;

	public MethodBuilder(string key, string value, DiagnosticsFactory diagnostics)
    {
        this.key = key;
        this.value = value;
		this.diagnostics = diagnostics;
	}

    public MethodModel Build(TypeModel target)
    {
        var parameters = BuildParameters(key);

        var methodNameWithoutParameters = key;

        foreach (var parameter in parameters)
        {
            methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, parameter.DisplayName);
        }

        string compilableMethodName = SanitizeMethodName(methodNameWithoutParameters.Trim());

        return new MethodModel(target, key, value, compilableMethodName, parameters, diagnostics);
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

    /// <summary>
    /// matches strings like {0}, {0:12}, {name} usable in format-strings
    /// </summary>
    internal static readonly Regex parameterExpression = new ("{(?<name>([0-9a-zA-Z]*))(:+(?<expression>[0-9a-zA-Z]*))?}");

    private IEnumerable<ParameterModel> BuildParameters(string rawValue)
    {
        var matches = parameterExpression.Matches(rawValue);

        if (matches.Count <= 0)
        {
            return Enumerable.Empty<ParameterModel>();
        }

        var parameters = new List<ParameterModel>(matches.Count);

        foreach (Match match in matches)
        {
            var token = match.Value;
			var name = match.Groups["name"].Value;
			var expression = match.Groups["expression"].Value;

			parameters.Add(new(token, name, expression, diagnostics));
        }

        return parameters
            .GroupBy(x => x.Name)
            .Select(x => x.First());
    }

}