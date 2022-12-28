using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class StringTypealizRClassBuilder
{
	private readonly string filePath;
	private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

	public StringTypealizRClassBuilder(string filePath, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		this.filePath = filePath;
		this.severityConfig = severityConfig;
	}

	private readonly List<MemberBuilderContext<InstanceMemberBuilder>> methodContexts = new();
	public StringTypealizRClassBuilder WithMember(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);
		methodContexts.Add(new (builder: new (key, value), diagnostics: new(diagnosticsFactory)));
		return this;
	}

	private readonly List<string> groups = new();

	public StringTypealizRClassBuilder WithGroup(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);

		if (!groups.Contains(key))
		{
			groups.Add(key);
		}

		return this;
	}

	public StringTypealizRClassModel Build(TypeModel target, string rootNamespace)
	{
		var members = methodContexts
			.Select(x => new MemberModelContext(x.Builder.Build(target, x.Diagnostics), x.Diagnostics))
			.ToArray()
		;

		var distinctMembers = members.Deduplicate();

		var allDiagnostics = members.SelectMany(x => x.Diagnostics.Entries);

		return new(target, rootNamespace, distinctMembers, groups, allDiagnostics);
    }

	
}
