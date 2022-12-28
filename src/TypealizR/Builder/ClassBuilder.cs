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
	private readonly string filePath;
	private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

	public ExtensionClassBuilder(string filePath, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		this.filePath = filePath;
		this.severityConfig = severityConfig;
	}

	private readonly List<MethodBuilderContext> methodContexts = new();
	public ExtensionClassBuilder Add(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);

		methodContexts.Add(new (builder: new(key, value), diagnostics: new(diagnosticsFactory)));
		return this;
	}

	public ClassModel Build(TypeModel target, string rootNamespace)
	{
		var methods = methodContexts
			.Select(x => new MethodModelContext(x.Builder.Build(target, x.Diagnostics), x.Diagnostics))
			.ToArray()
		;

		var distinctMethods = Deduplicate(methods);

		var allDiagnostics = methods.SelectMany(x => x.Diagnostics.Entries);

		return new(target, rootNamespace, distinctMethods, allDiagnostics);
    }

	private IEnumerable<MethodModel> Deduplicate(MethodModelContext[] methods)
	{
		var groupByMethodName = methods.GroupBy(x => x.Model.Name);
		var deduplicatedMethods = new List<MethodModelContext>(methods.Count());

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
				duplicate.Diagnostics.Add(fac => fac.AmbigiousRessourceKey_0002(duplicate.Model.Name));
				duplicate.Model.DeduplicateWith(discriminator++);
			}

			deduplicatedMethods.AddRange(methodGroup);
		}

		return deduplicatedMethods
			.Select(x => x.Model)
			.ToArray();

	}
}
