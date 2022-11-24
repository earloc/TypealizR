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
	public readonly IEnumerable<Diagnostic> Diagnostics;

	public ParameterModel(string token, string name, string annotation, DiagnosticsFactory factory)
    {
		Token = token;

		(Type, var invalidTypeAnnotation) = TryDeriveTypeFrom(annotation);

		var diagnostics = new List<Diagnostic>();
		if (!string.IsNullOrEmpty(invalidTypeAnnotation))
		{
			diagnostics.Add(factory.UnrecognizedParameterType_0004(invalidTypeAnnotation));
		}

		if (int.TryParse(name, out var _))
		{
			diagnostics.Add(factory.UnnamedGenericParameter_0003(name));
		}

		Name = SanitizeName(name);

        DisplayName = $"_{Name.Trim('_', ' ')}_";
		Declaration = $"{Type} {Name}";

		Diagnostics = diagnostics;
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
