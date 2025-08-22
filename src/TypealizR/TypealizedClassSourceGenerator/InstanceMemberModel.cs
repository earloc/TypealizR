using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Extensions;

namespace TypealizR;
internal class InstanceMemberModel : IMemberModel
{
    public void DeduplicateWith(int discriminator) => Name = new MemberName($"{Name}{discriminator}");


    private readonly string stringFormatterTypeName;
    private readonly ResourceKey resourceKey;
    private readonly string resourceDefaultValue;
    private readonly IEnumerable<ParameterModel> parameters;


    public InstanceMemberModel(string rootNameSpace, string rawRessourceName, string ressourceDefaultValue, MemberName name, IEnumerable<ParameterModel> parameters)
    {
        this.stringFormatterTypeName = StringFormatterClassBuilder.GlobalFullTypeName(rootNameSpace);
        this.resourceKey = new (rawRessourceName);
        this.resourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
        this.Name = name;
        this.parameters = parameters;
    }

    public MemberName Name { get; private set; }

    public string ToCSharp()
    {
        var signature = "";
        var body = $@"localizer[""{resourceKey}""]";

        if (parameters.Any())
        {
            var additionalParameterDeclarations = parameters.Select(x => $"{x.Type} {x.DisplayName}").ToCommaDelimited();
            signature = $"({additionalParameterDeclarations})";

            var parameterCollection = parameters.Select(x => x.DisplayName).ToCommaDelimited();
            body = $@"{stringFormatterTypeName}.Format(localizer[""{resourceKey}""], {parameterCollection})";
        }

        var comment = new CommentModel(resourceKey, resourceDefaultValue);

        return $"""

            {comment.ToCSharp()}
            public LocalizedString {Name}{signature}
                => {body};
    """;
    }
}
