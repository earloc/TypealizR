using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class ParameterModel
{
	public readonly string Token;
	public readonly string Type;
	public readonly string Name;
	public readonly string DisplayName;
	public readonly string Declaration;
	public readonly bool IsGeneric;
	public readonly string InvalidTypeAnnotation;
	public bool HasUnrecognizedParameterTypeAnnotation => !string.IsNullOrEmpty(InvalidTypeAnnotation);

	public ParameterModel(string token, string name, string annotation)
    {
		Token = token;

		(Type, InvalidTypeAnnotation) = TryDeriveTypeFrom(annotation);

		IsGeneric = int.TryParse(name, out var _);
		Name = SanitizeName(name);

        DisplayName = $"_{Name.Trim('_', ' ')}_";
		Declaration = $"{Type} {Name}";
	}

	private (string, string) TryDeriveTypeFrom(string expression)
	{
		if (string.IsNullOrEmpty(expression))
		{
			return ("object", "");
		}

		var type = SanitizeType(expression);
		if (type is not null)
		{
			return (type, "");
		}

		return ("object", expression);
	}

	private string? SanitizeType(string type) => type switch
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
		_ => null
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
