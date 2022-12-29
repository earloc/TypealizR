using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Builder;
internal class ExtensionMethodModel
{
    public void DeduplicateWith(int discriminator)
    {
        Name = $"{Name}{discriminator}";
    }

    public TypeModel ExtendedType { get; }
    public string RawRessourceName { get; }
    public readonly string RessourceDefaultValue;
    public string Name { get; private set; }

    public readonly IEnumerable<ParameterModel> Parameters;

    public ExtensionMethodModel(TypeModel extendedType, string rawRessourceName, string ressourceDefaultValue, string compilableMethodName, IEnumerable<ParameterModel> parameters)
    {
        ExtendedType = extendedType;
        RawRessourceName = rawRessourceName;
        RessourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
        Name = compilableMethodName;
        Parameters = parameters;
    }

    public string ToCSharp()
    {
        static string ThisParameterFor(TypeModel T) => $"this IStringLocalizer<{T.Name}> that";

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
