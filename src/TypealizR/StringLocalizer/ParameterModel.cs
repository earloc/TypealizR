using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.CodeAnalysis;
using TypealizR.Extensions;

namespace TypealizR.StringLocalizer;


internal class ParameterModel
{
	public readonly string Token;
	public readonly string Type;
	public readonly string Name;

	public ParameterModel(string token, string name)
    {
		Token = token;
		Name = SanitizeName(name);
	}

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
