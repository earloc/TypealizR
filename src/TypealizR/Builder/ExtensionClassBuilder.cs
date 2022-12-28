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

	private readonly List<MethodBuilderContext<ExtensionMethodBuilder>> methodContexts = new();
	public ExtensionClassBuilder Add(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);
		methodContexts.Add(new (builder: new(key, value), diagnostics: new(diagnosticsFactory)));
		return this;
	}

	public ExtensionClassModel Build(TypeModel target, string rootNamespace)
	{
		var methods = methodContexts
			.Select(x => new MethodModelContext(x.Builder.Build(target, x.Diagnostics), x.Diagnostics))
			.ToArray()
		;

		var distinctMethods = methods.Deduplicate();

		var allDiagnostics = methods.SelectMany(x => x.Diagnostics.Entries);

		return new(target, rootNamespace, distinctMethods, allDiagnostics);
    }
}
