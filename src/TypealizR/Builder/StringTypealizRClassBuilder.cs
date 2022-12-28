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
	private readonly string name;
	private readonly string filePath;
	private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

	public StringTypealizRClassBuilder(string name, string filePath, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		this.name = name;
		this.filePath = filePath;
		this.severityConfig = severityConfig;
	}

	private readonly List<MemberBuilderContext<InstanceMemberBuilder>> memberContexts = new();
	public StringTypealizRClassBuilder WithMember(string key, string value, int lineNumber)
	{
		var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);
		memberContexts.Add(new (builder: new (key, value), diagnostics: new(diagnosticsFactory)));
		return this;
	}

	private readonly Dictionary<string, StringTypealizRClassBuilder> nestedTypes = new();

	public StringTypealizRClassBuilder WithGroups(string key, IEnumerable<string> groupKeys, string value, int lineNumber)
	{
		//var diagnosticsFactory = new DiagnosticsFactory(filePath, key, lineNumber, severityConfig);

		if (!groupKeys.Any())
		{
			WithMember(key, value, lineNumber);
			return this;
		}

		var firstLevel = groupKeys.First();

		if (!nestedTypes.ContainsKey(firstLevel))
		{
			nestedTypes[firstLevel] = new StringTypealizRClassBuilder($"{firstLevel}", filePath, severityConfig);
		}

		nestedTypes[firstLevel].WithGroups(key, groupKeys.Skip(1), value, lineNumber);

		return this;
	}

	public StringTypealizRClassModel Build(TypeModel target, string rootNamespace)
	{
		var members = memberContexts
			.Select(x => new MemberModelContext(x.Builder.Build(target, x.Diagnostics), x.Diagnostics))
			.ToArray()
		;

		var distinctMembers = members.Deduplicate();

		var allDiagnostics = members.SelectMany(x => x.Diagnostics.Entries);

		var nested = nestedTypes.Values.Select(x => x.Build(target, rootNamespace)).ToArray();

		return new(name, target, rootNamespace, distinctMembers, nested, allDiagnostics);
    }

	
}
