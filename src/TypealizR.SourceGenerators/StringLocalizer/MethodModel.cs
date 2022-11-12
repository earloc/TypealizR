using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class MethodModel
{
    public static string ThisParameterFor(TypeInfo T) => $"this IStringLocalizer<{T.FullName}> that";

	internal void DeduplicateWith(int discriminator)
	{
        Name = $"{Name}{discriminator}";
	}

	public string RawRessourceName { get; }
    private readonly string defaultValue;
    public string Name;
    public readonly IEnumerable<ParameterModel> Parameters;
    public readonly string Signature;
    public readonly string Body;
    public readonly string ReturnType = "LocalizedString";
    public int LineNumber { get; }

    public MethodModel(TypeInfo t, string rawRessourceName, string defaultValue, string compilableMethodName, int lineNumber, IEnumerable<ParameterModel>? parameters = null)
    {
		RawRessourceName = rawRessourceName;
        this.defaultValue = defaultValue;
        Name = compilableMethodName;
		LineNumber = lineNumber;
		Parameters = parameters ?? Enumerable.Empty<ParameterModel>();

        Signature = $"({ThisParameterFor(t)})";
        Body = $@"that[""{rawRessourceName}""]";

        
        if (Parameters.Any())
        {
			var additionalParameterDeclarations = string.Join(", ", Parameters.Select(x => x.Declaration));
            Signature = $"({ThisParameterFor(t)}, {additionalParameterDeclarations})";

            var parameterCollection = string.Join(", ", Parameters.Select(x => x.Name));
            Body = $@"that[""{rawRessourceName}"", {parameterCollection}]";
        }
    }

    public string Declaration => $@"  
  /// <summary>
  /// Looks up a localized string similar to '{RawRessourceName}'
  /// </summary>
  /// <returns>
  /// A localized version of the current default value of '{defaultValue.Replace("\r\n", " ").Replace("\n", " ")}'
  /// </returns>
  public static {ReturnType} {Name}{Signature} => {Body};
";

}
