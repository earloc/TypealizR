using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using static System.Reflection.Metadata.Ecma335.MethodBodyStreamEncoder;

namespace TypealizR.StringLocalizer;
internal class IStringLocalizerExtensionMethodWriter
{
	private readonly MethodModel model;

	public IStringLocalizerExtensionMethodWriter(MethodModel model)
	{
		this.model = model;
	}

	public string ToCSharp()
    {
		static string ThisParameterFor(TypeModel T) => $"this IStringLocalizer<{T.FullName}> that";

		var signature = $"({ThisParameterFor(model.ExtendedType)})";
		var body = $@"that[""{model.RawRessourceName}""]";

		if (model.Parameters.Any())
		{
			var additionalParameterDeclarations = string.Join(", ", model.Parameters.Select(x => x.Declaration));
			signature = $"({ThisParameterFor(model.ExtendedType)}, {additionalParameterDeclarations})";

			var parameterCollection = model.Parameters.Select(x => x.Name).ToCommaDelimited();
			body = body = $@"that[""{model.RawRessourceName}""].Format({parameterCollection})";
		}

		return $@"  
          /// <summary>
          /// Looks up a localized string similar to '{model.RawRessourceName}'
          /// </summary>
          /// <returns>
          /// A localized version of the current default value of '{model.RessourceDefaultValue}'
          /// </returns>
          public static {model.ReturnType} {model.Name} {signature} => {body};
";
    }
}
