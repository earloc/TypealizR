using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;

internal class MethodBuilder
{
    private string key;
    private readonly string value;
	private readonly int lineNumber;

	public MethodBuilder(string key, string value, int lineNumber)
    {
        this.key = key;
        this.value = value;
		this.lineNumber = lineNumber;
	}

    public MethodModel Build(TypeInfo target)
    {
        var parameters = BuildParameters(key);

        var methodNameWithoutParameters = key;

        foreach (var parameter in parameters)
        {
            methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, parameter.DisplayName);
        }

        string compilableMethodName = SanitizeMethodName(methodNameWithoutParameters.Trim());

        return new MethodModel(target, key, value, compilableMethodName, lineNumber, parameters);
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
    internal static Regex parameterExpression = new ("{(?<name>([0-9a-zA-Z]*))(:+(?<expression>[0-9a-zA-Z]*))?}");

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

			parameters.Add(new(token, name, expression));
        }

        return parameters
            .GroupBy(x => x.Name)
            .Select(x => x.First());
    }

}