using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class ExtensionMethodParameterInfo
{
	public readonly string Token;
	public readonly string Type;
	public readonly string Name;
	public readonly string DisplayName;
	public readonly string Declaration;
	public readonly bool IsGeneric;

	public ExtensionMethodParameterInfo(string token)
    {
        Token = token;
        Type = "object";

        var content = token.Trim('{', '}');
		Name = SanitizeParameterName(content);
		IsGeneric = int.TryParse(Name, out var _);

        DisplayName = $"_{Name.Trim('_', ' ')}_";
		Declaration = $"{Type} {Name}";
	}

	private string SanitizeParameterName(string rawParameterName)
    {
        var parameterName = new string(
            rawParameterName
                .Replace(" ", "_")
                .ToArray()
        );

        if (!parameterName.First().IsValidInIdentifier())
        {
            return $"_{rawParameterName}";
        }

        return parameterName;
    }
}
