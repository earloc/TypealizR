using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.StringLocalizer;
internal class MethodModel
{

	internal void DeduplicateWith(int discriminator)
	{
        Name = $"{Name}{discriminator}";
		diagnostics.Add(factory.AmbigiousRessourceKey_0002(Name));
	}

    private readonly List<Diagnostic> diagnostics = new();
    public IEnumerable<Diagnostic> Diagnostics => diagnostics;


	public TypeModel ExtendedType { get; }
	public string RawRessourceName { get; }
	public readonly string RessourceDefaultValue;
    public string Name;
    public readonly IEnumerable<ParameterModel> Parameters;
	
    public readonly string ReturnType = "LocalizedString";

    public MethodModel(TypeModel extendedType, string rawRessourceName, string ressourceDefaultValue, string compilableMethodName, IEnumerable<ParameterModel> parameters)
    {
		ExtendedType = extendedType;
		RawRessourceName = rawRessourceName;
        RessourceDefaultValue = ressourceDefaultValue.Replace("\r\n", " ").Replace("\n", " ");
        Name = compilableMethodName;
		Parameters = parameters ?? Enumerable.Empty<ParameterModel>();
    }
}
