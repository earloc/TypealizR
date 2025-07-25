﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Extensions;

namespace TypealizR;
internal class ExtensionMethodModel : IMemberModel
{
    public void DeduplicateWith(int discriminator) => Name = new MemberName($"{Name}{discriminator}");

    public TypeModel ExtendedType { get; }
    public string RawRessourceName { get; }
    public readonly string RessourceDefaultValue;
    public MemberName Name { get; private set; }

    public readonly IEnumerable<ParameterModel> Parameters;

    private readonly string stringFormatterTypeName;

    public ExtensionMethodModel(string rootNamespace, TypeModel extendedType, string rawRessourceName, string ressourceDefaultValue, MemberName name, IEnumerable<ParameterModel> parameters)
    {
        ExtendedType = extendedType;
        RawRessourceName = rawRessourceName;
        RessourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
        Name = name;
        Parameters = parameters;
        stringFormatterTypeName = StringFormatterClassBuilder.GlobalFullTypeName(rootNamespace);
    }

    public string ToCSharp()
    {
        static string ThisParameterFor(TypeModel T) => $"this IStringLocalizer<{T.GlobalFullName}> that";

        var constName = $"_{Name}";
        var signature = $"({ThisParameterFor(ExtendedType)})";
        var body = $@"that[{constName}]";

        if (Parameters.Any())
        {
            var additionalParameterDeclarations = Parameters.Select(x => $"{x.Type} {x.DisplayName}").ToCommaDelimited();
            signature = $"({ThisParameterFor(ExtendedType)}, {additionalParameterDeclarations})";

            var parameterCollection = Parameters.Select(Invocation).ToCommaDelimited();

            body = $"{stringFormatterTypeName}.Format(that[{constName}], {parameterCollection})";
        }
        var comment = new CommentModel(RawRessourceName, RessourceDefaultValue);

        return $$"""

            {{comment.ToCSharp()}}
            [DebuggerStepThrough]
            public static LocalizedString {{Name}}{{signature}} => {{body}};
            private const string {{constName}} = "{{RawRessourceName}}";
    """;
    }

    private string Invocation(ParameterModel parameter) => string.IsNullOrEmpty(parameter.Extension) 
        ? parameter.DisplayName 
        : $"""{stringFormatterTypeName}.Extend({parameter.DisplayName}, "{parameter.Extension}")"""
    ;
}
