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

	public ExtensionMethodParameterInfo(string token, string name, string expression)
    {
        Token = token;
        Type = "object";

		if (!string.IsNullOrEmpty(expression))
        {
            Type = SanitizeType(expression);
        }

		IsGeneric = int.TryParse(name, out var _);
		Name = SanitizeName(name);

        DisplayName = $"_{Name.Trim('_', ' ')}_";
		Declaration = $"{Type} {Name}";
	}

    private string SanitizeType(string type) => type switch
    {
        "int" => "int",
		"i" => "int",
		"string" => "string",
		"s" => "string",
		"DateTime" => "DateTime",
		"dt" => "DateTime",
		"DateTimeOffset" => "DateTimeOffset",
		"dto" => "DateTimeOffset",
		"DateOnly" => "DateOnly",
		"d" => "DateOnly",
		"TimeOnly" => "TimeOnly",
		"t" => "TimeOnly",
		_ => "object"
    };
	
	private string SanitizeName(string rawParameterName)
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
