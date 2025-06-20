﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using TypealizR.Core.Diagnostics;

namespace TypealizR.Core;
internal class ParameterBuilder
{
    public ParameterBuilder(string rawKeyValue) => this.rawKeyValue = rawKeyValue;

    private readonly List<ParameterModel> models = [];
    private readonly string rawKeyValue;

    internal IEnumerable<ParameterModel> Build(DiagnosticsCollector diagnostics)
    {
        var segments = rawKeyValue.Split('}').Select(x => $$"""{{x}}}""");
        foreach (var segment in segments) {
            var matches = parameterExpression.Matches(segment);

            if (matches.Count <= 0)
            {
                continue;
            }

            foreach (Match match in matches)
            {
                var token = match.Value;
                var name = match.Groups["name"].Value;
                var annotation = match.Groups["annotation"].Value.Split('@')[0];
                var extension = match.Groups["extension"].Value;

                (var type, var invalidTypeAnnotation) = TryDeriveTypeFrom(annotation);

                if (!string.IsNullOrEmpty(invalidTypeAnnotation))
                {
                    diagnostics.Add(x => x.UnrecognizedParameterType_0004(invalidTypeAnnotation));
                }
                if (int.TryParse(name, out var _))
                {
                    diagnostics.Add(x => x.UnnamedGenericParameter_0003(name));
                }

                models.Add(new(token, name, type, extension));
            }
        }
        return models
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToArray()
        ;
    }

    private static (string, string) TryDeriveTypeFrom(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            return ("object", "");
        }

        var type = SanitizeType(expression);
        return type is not null ? ((string, string))(type, "") : ((string, string))("object", expression);
    }

    private static string? SanitizeType(string type)
    {
        type = type.ToUpperInvariant();

        if (ParameterAnnotation.Mappings.TryGetValue(type, out var targetType))
        {
            return targetType;
        }

        return null;
    }

    /// <summary>
    /// matches strings like {0}, {0:12}, {name} usable in format-strings
    /// </summary>
    internal static readonly Regex parameterExpression = new("{(?<name>([0-9a-zA-Z]*))(:+(?<annotation>[0-9a-zA-Z]*))?(@+(?<extension>.*))?}", RegexOptions.None, TimeSpan.FromMilliseconds(100));
}
