using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Builder;
internal class InstanceMemberModel : IMemberModel
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

	public InstanceMemberModel(TypeModel extendedType, string rawRessourceName, string ressourceDefaultValue, string compilableMethodName, IEnumerable<ParameterModel> parameters)
	{
		ExtendedType = extendedType;
		RawRessourceName = rawRessourceName;
		RessourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
		Name = compilableMethodName;
		Parameters = parameters;
	}

	public string ToCSharp()
	{
		var signature = "";
		var body = $@"localizer[""{RawRessourceName}""]";

		if (Parameters.Any())
		{
			var additionalParameterDeclarations = string.Join(", ", Parameters.Select(x => $"{x.Type} {x.DisplayName}"));
			signature = $"({additionalParameterDeclarations})";

			var parameterCollection = Parameters.Select(x => x.DisplayName).ToCommaDelimited();
			body = $@"localizer[""{RawRessourceName}""].Format({parameterCollection})";
		}

		var comment = new CommentModel(RawRessourceName, RessourceDefaultValue);

		return $"""
	{comment.ToCSharp()}
			public LocalizedString {Name}{signature}
				=> {body};
	""";
	}
}
