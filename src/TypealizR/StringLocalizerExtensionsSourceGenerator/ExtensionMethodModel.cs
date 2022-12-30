﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Values;namespace TypealizR.Builder;
internal class ExtensionMethodModel : IMemberModel
{
    public void DeduplicateWith(int discriminator)
    {
        Name = new MemberName($"{Name}{discriminator}");
    }

    public TypeModel ExtendedType { get; }
    public string RawRessourceName { get; }
    public readonly string RessourceDefaultValue;
    public MemberName Name { get; private set; }

    public readonly IEnumerable<ParameterModel> Parameters;

    public ExtensionMethodModel(TypeModel extendedType, string rawRessourceName, string ressourceDefaultValue, MemberName name, IEnumerable<ParameterModel> parameters)
    {
        ExtendedType = extendedType;
        RawRessourceName = rawRessourceName;
        RessourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
        Name = name;
        Parameters = parameters;
    }

    public string ToCSharp()
    {
        static string ThisParameterFor(TypeModel T) => $"this IStringLocalizer<{T.GlobalFullName}> that";

        var signature = $"({ThisParameterFor(ExtendedType)})";
        var body = $@"that[""{RawRessourceName}""]";

        if (Parameters.Any())
        {
            var additionalParameterDeclarations = string.Join(", ", Parameters.Select(x => $"{x.Type} {x.DisplayName}"));
            signature = $"({ThisParameterFor(ExtendedType)}, {additionalParameterDeclarations})";

            var parameterCollection = Parameters.Select(x => x.DisplayName).ToCommaDelimited();
            body = $@"that[""{RawRessourceName}""].Format({parameterCollection})";
        }
        var comment = new CommentModel(RawRessourceName, RessourceDefaultValue);

        return $"""
            {comment.ToCSharp()}
            [DebuggerStepThrough]
            public static LocalizedString {Name}{signature}
                => {body};
    """;
    }
}
