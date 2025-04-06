using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypealizR.Extensions;

namespace TypealizR;
internal class StringFormatterClassBuilder
{
    internal const string TypeName = "TypealizR_StringFormatter";

    private readonly string rootNamespace;
    private readonly bool supportsDateAndTimeOnly;

    public StringFormatterClassBuilder(string rootNamespace, bool supportsDateAndTimeOnly)
    {
        this.rootNamespace = rootNamespace;
        this.supportsDateAndTimeOnly = supportsDateAndTimeOnly;
    }

    private bool formatMethodExists;

    private HashSet<string> alreadyExtendedTypes = [];
    internal void UserModeTypeExists(bool formatMethodExists, HashSet<string> alreadyExtendedTypes)
    {
        this.alreadyExtendedTypes = alreadyExtendedTypes;
        this.formatMethodExists = formatMethodExists;
    } 

    internal string Build(Type generatorType)
    {
        var stringFormatterStub = GenerateStub(generatorType, supportsDateAndTimeOnly);

        string defaultImplementation = "";
        if (!formatMethodExists || alreadyExtendedTypes.Count != ParameterAnnotation.NetStandard20SupportedTypes.Count)
        {
            defaultImplementation = GenerateDefaultImplementation(formatMethodExists, alreadyExtendedTypes, supportsDateAndTimeOnly);
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
        using System;
        using System.Diagnostics;
        using System.CodeDom.Compiler;
        using Microsoft.Extensions.Localization;
        
        """;

    private static string OpenNamespace(string rootNamespace) => $@"namespace {rootNamespace} {{";

    private static string GenerateArgumentExtensionOverloads(string body, HashSet<string> userModeImplementedTypes, bool supportsDateAndTimeOnly)
    {
        var builder = new StringBuilder();

        var typeCandidates = supportsDateAndTimeOnly ? ParameterAnnotation.Net60SupportedTypes : ParameterAnnotation.NetStandard20SupportedTypes;

        foreach (var annotationType in typeCandidates.Keys.Where(x => !userModeImplementedTypes.Contains(x)))
        {
            builder.AppendLine($$"""

                    internal static partial {{annotationType}} Extend(this {{annotationType}} argument, string extension){{body}}
            """);
        }

        return builder.ToString();
    }
    private static string GenerateStub(Type generatorType, bool supportsDateAndTimeOnly) => $$"""
        {{generatorType.GeneratedCodeAttribute()}}
        internal static partial class {{TypeName}}
        {
            [DebuggerStepThrough]
            internal static LocalizedString Format(this LocalizedString that, params object[] args) => new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

            internal static LocalizedString Or(this LocalizedString that, LocalizedString fallBack) => that.ResourceNotFound ? fallBack : that;

            internal static partial string Format(string s, object[] args);
            {{GenerateArgumentExtensionOverloads(";", [], supportsDateAndTimeOnly)}}
        }
    """;

    private static string GenerateDefaultImplementation(bool formatMethodExists, HashSet<string> userModeImplementedTypes, bool supportsDateAndTimeOnly)
    {
        var formatImplementation = formatMethodExists ? "" : "internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);";
        return $$"""

            internal static partial class {{TypeName}}
            {
                {{formatImplementation}}

                {{GenerateArgumentExtensionOverloads(" => argument;", userModeImplementedTypes, supportsDateAndTimeOnly)}}
            }
        """;
    }

    private const string CloseNamespace = "}";
}

