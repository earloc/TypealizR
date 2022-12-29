﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Builder;
internal class InstanceMemberModel
{
	public void DeduplicateWith(int discriminator)
	{
		Name = $"{Name}{discriminator}";
	}


	private readonly string rawRessourceName;
	private readonly string ressourceDefaultValue;
	private readonly IEnumerable<ParameterModel> parameters;

	public InstanceMemberModel(string rawRessourceName, string ressourceDefaultValue, string compilableMethodName, IEnumerable<ParameterModel> parameters)
	{
		this.rawRessourceName = rawRessourceName;
		this.ressourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
		this.Name = compilableMethodName;
		this.parameters = parameters;
	}

	public string Name { get; private set; }


	public string ToCSharp()
	{
		var signature = "";
		var body = $@"localizer[""{rawRessourceName}""]";

		if (parameters.Any())
		{
			var additionalParameterDeclarations = string.Join(", ", parameters.Select(x => $"{x.Type} {x.DisplayName}"));
			signature = $"({additionalParameterDeclarations})";

			var parameterCollection = parameters.Select(x => x.DisplayName).ToCommaDelimited();
			body = $@"localizer[""{rawRessourceName}""].Format({parameterCollection})";
		}

		var comment = new CommentModel(rawRessourceName, ressourceDefaultValue);

		return $"""
	{comment.ToCSharp()}
			public LocalizedString {Name}{signature}
				=> {body};
	""";
	}
}
