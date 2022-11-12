using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class TypeInfo
{
	public TypeInfo(string @namespace, string name)
	{
		Namespace = @namespace;
		Name = name;
	}

    public string Namespace { get; }
    public string Name { get; }

	public string FullName => $"{Namespace}.{Name}";

}
internal class StringLocalizerExtensionClassBuilder
{

	public StringLocalizerExtensionClassBuilder(string fileName)
	{
		this.fileName = fileName;
	}

	private List<StringLocalizerExtensionMethodBuilder> methodBuilders = new();
	private readonly string fileName;

	public StringLocalizerExtensionClassBuilder WithMethodFor(string key, string value, int lineNumber)
	{
        methodBuilders.Add(new(key, value, lineNumber));
		return this;
	}

	public ExtensionClassInfo Build(TypeInfo target)
	{
		var methods = methodBuilders
			.Select(x => x.Build(target))
			.ToArray()
		;

		var deduplicated = Deduplicate(fileName, methods);

		var parameterWarnings = deduplicated
			.Methods
			.SelectMany(method =>
				method.Parameters
				.Where(parameter => parameter.IsGeneric)
				.Select(parameter => ErrorCodes.UnnamedGenericParameter_001011(fileName, method.LineNumber, method.RawRessourceName, parameter.Token))
			);

		var allWarnings = deduplicated.Warnings.Concat(parameterWarnings);

		return new(target, deduplicated.Methods, allWarnings);
    }

	private (IEnumerable<ExtensionMethodInfo> Methods, IEnumerable<Diagnostic> Warnings) Deduplicate(string fileName, ExtensionMethodInfo[] methods)
	{
		var groupByMethodName = methods.GroupBy(x => x.Name);
		var deduplicatedMethods = new List<ExtensionMethodInfo>(methods.Count());
		var warnings = new List<Diagnostic>(methods.Count());

		foreach (var methodGroup in groupByMethodName)
		{
			if (methodGroup.Count() == 1)
			{
				deduplicatedMethods.Add(methodGroup.Single());
				continue;
			}

			int discriminator = 1;
			foreach (var duplicate in methodGroup.Skip(1))
			{
				duplicate.DeduplicateWith(discriminator++);
				warnings.Add(ErrorCodes.AmbigiousRessourceKey_001010(fileName, duplicate.LineNumber, duplicate.RawRessourceName, duplicate.Name));
			}

			deduplicatedMethods.AddRange(methodGroup);
		}

		return (deduplicatedMethods, warnings);

	}

	public IEnumerable<StringLocalizerExtensionMethodBuilder> Methods => methodBuilders;

}
