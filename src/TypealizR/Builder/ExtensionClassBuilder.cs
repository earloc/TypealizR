using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class ExtensionClassBuilder
{
	private readonly TypeModel markerType;
	private readonly string rootNamespace;

	public ExtensionClassBuilder(TypeModel markerType, string rootNamespace)
	{
		this.markerType = markerType;
		this.rootNamespace = rootNamespace;
	}

	private Dictionary<string, ExtensionMethodModel> methods = new();
	private Dictionary<string, int> duplicates = new();

	public ExtensionClassBuilder WithExtensionMethod(string key, string value, DiagnosticsCollector diagnostics)
	{
		var builder = new ExtensionMethodBuilder(markerType, key, value, diagnostics);
		var model = builder.Build();

		if (!duplicates.ContainsKey(model.Name))
		{
			duplicates[model.Name] = 1;
		}
		else
		{
			var discriminator = duplicates[model.Name]++;
			model.DeduplicateWith(discriminator);
			diagnostics.Add(fac => fac.AmbigiousRessourceKey_0002(model.Name));
		}

		methods[model.Name] = model;
		return this;
	}

	public ExtensionClassModel Build()
	{
		return new(markerType, rootNamespace, methods.Values);
    }
}
