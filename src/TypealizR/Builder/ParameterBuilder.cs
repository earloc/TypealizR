using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal class ParameterBuilder
{
	public ParameterBuilder(string rawKeyValue)
	{
		this.rawKeyValue = rawKeyValue;
	}

	private readonly List<ParameterModel> models = new ();
	private readonly string rawKeyValue;

	internal IEnumerable<ParameterModel> Build(DiagnosticsCollector diagnostics)
	{
		var matches = parameterExpression.Matches(rawKeyValue);

		if (matches.Count <= 0)
		{
			return Enumerable.Empty<ParameterModel>();
		}

		foreach (Match match in matches)
		{
			var token = match.Value;
			var name = match.Groups["name"].Value;
			var annotation = match.Groups["annotation"].Value;

			(var type, var invalidTypeAnnotation) = TryDeriveTypeFrom(annotation);
			
			if (!string.IsNullOrEmpty(invalidTypeAnnotation))
			{
				diagnostics.Add(x => x.UnrecognizedParameterType_0004(invalidTypeAnnotation));
			}
			if (int.TryParse(name, out var _))
			{
				diagnostics.Add(x => x.UnnamedGenericParameter_0003(name));
			}

			models.Add(new(token, name, type));
		}

		return models
			.GroupBy(x => x.Name)
			.Select(x => x.First())
			.ToArray()
		;
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

	/// <summary>
	/// matches strings like {0}, {0:12}, {name} usable in format-strings
	/// </summary>
	internal static readonly Regex parameterExpression = new("{(?<name>([0-9a-zA-Z]*))(:+(?<annotation>[0-9a-zA-Z]*))?}", RegexOptions.None, TimeSpan.FromMilliseconds(100));
}
