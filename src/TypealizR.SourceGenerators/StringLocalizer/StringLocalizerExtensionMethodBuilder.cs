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

internal class StringLocalizerExtensionMethodBuilder
{
    private string key;
    private readonly string value;
	private readonly int lineNumber;

	public StringLocalizerExtensionMethodBuilder(string key, string value, int lineNumber)
    {
        this.key = key;
        this.value = value;
		this.lineNumber = lineNumber;
	}

    public ExtensionMethodInfo Build(TypeInfo target)
    {
        var parameters = BuildParameters(key);

        var methodNameWithoutParameters = key;

        foreach (var parameter in parameters)
        {
            methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, parameter.DisplayName);
        }

        string compilableMethodName = SanitizeMethodName(methodNameWithoutParameters.Trim());

        return new ExtensionMethodInfo(target, key, value, compilableMethodName, lineNumber, parameters);
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
    /// catches values '0', '0:12', 'name' in the 'content'-group for further processing
    /// </summary>
    static Regex parameterExpression = new ("{(?<content>\\d(:+.*)?)}|{(?<content>[a-zA-Z](\\d|\\w)*)}");

    private IEnumerable<ExtensionMethodParameterInfo> BuildParameters(string rawValue)
    {
        var matches = parameterExpression.Matches(rawValue);

        if (matches.Count <= 0)
        {
            return Enumerable.Empty<ExtensionMethodParameterInfo>();
        }

        var parameters = new List<ExtensionMethodParameterInfo>(matches.Count);

        foreach (Match match in matches)
        {
            var token = match.Value;
            var content = match.Groups["content"].Value;

            parameters.Add(new(token, content));
        }

        return parameters
            .GroupBy(x => x.Name)
            .Select(x => x.First());
    }

}