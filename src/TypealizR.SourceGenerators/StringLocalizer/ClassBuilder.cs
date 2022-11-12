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
internal class ClassBuilder
{

	public ClassBuilder(string fileName)
	{
		this.fileName = fileName;
	}

	private readonly List<MethodBuilder> methodBuilders = new();
	private readonly string fileName;

	public ClassBuilder WithMethodFor(string key, string value, int lineNumber)
	{
        methodBuilders.Add(new(key, value, lineNumber));
		return this;
	}

	public ClassModel Build(TypeInfo target)
	{
		var methods = methodBuilders
			.Select(x => x.Build(target))
			.ToArray()
		;

		var deduplicated = Deduplicate(fileName, methods);

		var genericParameterWarnings = deduplicated
			.Methods
			.SelectMany(method =>
				method.Parameters
				.Where(parameter => parameter.IsGeneric)
				.Select(parameter => ErrorCodes.UnnamedGenericParameter_0003(fileName, method.RawRessourceName, method.LineNumber, parameter.Token))
		);

		var unrecognizedParameterTypeWarnings = deduplicated
			.Methods
			.SelectMany(method =>
				method.Parameters
				.Where(parameter => parameter.HasUnrecognizedParameterTypeAnnotation)
				.Select(parameter => ErrorCodes.UnrecognizedParameterType_0004(fileName, method.RawRessourceName, method.LineNumber, parameter.InvalidTypeAnnotation))
		);

		var allWarnings = deduplicated.Warnings
			.Concat(genericParameterWarnings)
			.Concat(unrecognizedParameterTypeWarnings)
		;

		return new(target, deduplicated.Methods, allWarnings);
    }

	private (IEnumerable<MethodModel> Methods, IEnumerable<Diagnostic> Warnings) Deduplicate(string fileName, MethodModel[] methods)
	{
		var groupByMethodName = methods.GroupBy(x => x.Name);
		var deduplicatedMethods = new List<MethodModel>(methods.Count());
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
				warnings.Add(ErrorCodes.AmbigiousRessourceKey_0002(fileName, duplicate.RawRessourceName, duplicate.LineNumber, duplicate.Name));
			}

			deduplicatedMethods.AddRange(methodGroup);
		}

		return (deduplicatedMethods, warnings);

	}

	public IEnumerable<MethodBuilder> Methods => methodBuilders;

}
