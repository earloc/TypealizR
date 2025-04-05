using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypealizR.Extensions;

namespace TypealizR;
internal class StringFormatterClassBuilder
{
    internal const string TypeName = "TypealizR_StringFormatter";

    private readonly string rootNamespace;

    public StringFormatterClassBuilder(string rootNamespace) => this.rootNamespace = rootNamespace;

    private bool formatMethodExists;

    private HashSet<string> alreadyExtendedTypes = [];
    internal void UserModeTypeExists(bool formatMethodExists, HashSet<string> alreadyExtendedTypes)
    {
        this.alreadyExtendedTypes = alreadyExtendedTypes;
        this.formatMethodExists = formatMethodExists;
    } 

    internal string Build(Type generatorType)
    {
        var stringFormatterStub = GenerateStub(generatorType);

        string defaultImplementation = "";
        if (!formatMethodExists || alreadyExtendedTypes.Count != ParameterAnnotation.SupportedTypes.Count)
        {
            defaultImplementation = GenerateDefaultImplementation(formatMethodExists, alreadyExtendedTypes);
        }

        var builder = new StringBuilder();

        builder.Append(GenerateUsings());
        builder.AppendLine(OpenNamespace(this.rootNamespace));

        builder.AppendLine();
        builder.AppendLine(stringFormatterStub);

        if (!string.IsNullOrEmpty(defaultImplementation))
        {
            builder.AppendLine();
            builder.AppendLine(defaultImplementation);
        }

        builder.AppendLine(CloseNamespace);
        builder.AppendLine();

        return builder.ToString();
    }

    private static string GenerateUsings() => $"""
        using System.Diagnostics;
        using System.CodeDom.Compiler;
        using Microsoft.Extensions.Localization;

        """;

    private static string OpenNamespace(string rootNamespace) => $@"namespace {rootNamespace} {{";

    private static string GenerateArgumentExtensionOverloads(string body, HashSet<string> userModeImplementedTypes)
    {
        var builder = new StringBuilder();
        foreach (var annotationType in ParameterAnnotation.SupportedTypes.Keys.Where(x => !userModeImplementedTypes.Contains(x)))
        {
            builder.AppendLine($$"""

                    internal static partial {{annotationType}} Extend(this {{annotationType}} argument, string extension){{body}}
            """);
        }

        return builder.ToString();
    }
    private static string GenerateStub(Type generatorType) => $$"""
        {{generatorType.GeneratedCodeAttribute()}}
        internal static partial class {{TypeName}}
        {
            [DebuggerStepThrough]
            internal static LocalizedString Format(this LocalizedString that, params object[] args) => new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

            internal static LocalizedString Or(this LocalizedString that, LocalizedString fallBack) => that.ResourceNotFound ? fallBack : that;

            internal static partial string Format(string s, object[] args);
            {{GenerateArgumentExtensionOverloads(";", [])}}
        }
    """;

    private static string GenerateDefaultImplementation(bool formatMethodExists, HashSet<string> userModeImplementedTypes)
    {
        var formatImplementation = formatMethodExists ? "" : "internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);";
        return $$"""

            internal static partial class {{TypeName}}s
            {
                {{formatImplementation}}

                {{GenerateArgumentExtensionOverloads(" => argument;", userModeImplementedTypes)}}
            }
        """;
    }

    private const string CloseNamespace = "}";
}

