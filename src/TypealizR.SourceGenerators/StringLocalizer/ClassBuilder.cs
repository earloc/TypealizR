using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class ClassBuilder
{
	private readonly string filePath;
	private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

	public ClassBuilder(string filePath, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		this.filePath = filePath;
		this.severityConfig = severityConfig;
	}

	private readonly List<MethodBuilder> methodBuilders = new();
	public ClassBuilder WithMethodFor(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);
        methodBuilders.Add(new(key, value, diagnosticsFactory));
		return this;
	}

	public ClassModel Build(TypeModel target)
	{
		var methods = methodBuilders
			.Select(x => x.Build(target))
			.ToArray()
		;

		var distinctMethods = Deduplicate(filePath, methods);

		var parameterDiagnostics = distinctMethods
			.SelectMany(method =>
				method.Parameters
				.SelectMany(parameter => parameter.Diagnostics)
		);

		var allWarningsAndErrors = distinctMethods.SelectMany(x => x.Diagnostics)
			.Concat(parameterDiagnostics)
		;

		return new(target, distinctMethods, allWarningsAndErrors);
    }

	private IEnumerable<MethodModel> Deduplicate(string fileName, MethodModel[] methods)
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
			}

			deduplicatedMethods.AddRange(methodGroup);
		}

		return deduplicatedMethods;

	}

	public IEnumerable<MethodBuilder> Methods => methodBuilders;

}
