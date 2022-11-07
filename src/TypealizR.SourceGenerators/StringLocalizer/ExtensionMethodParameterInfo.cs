using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class ExtensionMethodParameterInfo
{
    public ExtensionMethodParameterInfo(string token, string content)
    {
        Token = token;
        Type = "object";
        Name = SanitizeParameterName(content);
    }

    public readonly string Token;
    public readonly string Type;
    public readonly string Name;
    public string DisplayName => $"_{Name.Trim('_', ' ')}_";
    public string Declaration => $"{Type} {Name}";

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
