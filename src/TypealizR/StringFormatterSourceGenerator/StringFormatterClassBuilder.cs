﻿using System;
using System.Text;
using TypealizR.Extensions;

namespace TypealizR;
internal class StringFormatterClassBuilder
{
    internal const string TypeName = "TypealizR_StringFormatter";

    private readonly string rootNamespace;

    public StringFormatterClassBuilder(string rootNamespace) => this.rootNamespace = rootNamespace;

    private bool isUserModeImplementationProvided;
    internal void UserModeImplementationIsProvided() => isUserModeImplementationProvided = true;

    internal string Build(Type generatorType)
    {
        var stringFormatterStub = GenerateStub(generatorType);

        string? defaultImplementation = default;
        if (isUserModeImplementationProvided)
        {
            defaultImplementation = GenerateDefaultImplementation();
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

    private static string GenerateStub(Type generatorType) => $$"""
        {{generatorType.GeneratedCodeAttribute()}}
        internal static partial class {{TypeName}}
        {
                [DebuggerStepThrough]
                internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
                    new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

                [DebuggerStepThrough]
                internal static object Extend(this object that, string extension) => ExtendArg(that, extension);

                internal static LocalizedString Or(this LocalizedString that, LocalizedString fallBack) => 
                    that.ResourceNotFound ? fallBack : that;

            internal static partial string Format(string s, object[] args);
            internal static partial object ExtendArg(object arg, string annotationExtension);
        }
    """;

    private static string GenerateDefaultImplementation() => $$"""
        internal static partial class {{TypeName}} {
            [DebuggerStepThrough]
            internal static partial string Format(string s, object[] args) => 
                string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);

            [DebuggerStepThrough]
            internal static partial object ExtendArg(object arg, string annotationExtension) => arg;
        }
    """;

    private const string CloseNamespace = "}";
}

