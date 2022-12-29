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
	private readonly TypeModel markerType;
	private readonly string name;
	private readonly string rootNamespace;
	private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

	public StringTypealizRClassBuilder(TypeModel markerType, string name, string rootNamespace, IDictionary<string, DiagnosticSeverity> severityConfig)
	{
		this.markerType = markerType;
		this.name = name;
		this.rootNamespace = rootNamespace;
		this.severityConfig = severityConfig;
	}

	private readonly List<InstanceMemberModel> members = new();

	public StringTypealizRClassBuilder WithMember(string key, string value, DiagnosticsCollector diagnostics)
	{
		var builder = new InstanceMemberBuilder(key, value);
		var model = builder.Build(diagnostics);

		members.Add(model);

		return this;
	}

	private readonly Dictionary<string, StringTypealizRClassBuilder> nestedTypes = new();

	public StringTypealizRClassBuilder WithGroups(string key, IEnumerable<string> groupKeys, string value, DiagnosticsCollector diagnostics)
	{
		if (!groupKeys.Any())
		{
			WithMember(key, value, diagnostics);
			return this;
		}

		var firstLevel = groupKeys.First();

		if (!nestedTypes.ContainsKey(firstLevel))
		{
			nestedTypes[firstLevel] = new StringTypealizRClassBuilder(markerType, $"{firstLevel}", rootNamespace, severityConfig);
		}

		nestedTypes[firstLevel].WithGroups(key, groupKeys.Skip(1), value, diagnostics);

		return this;
	}

	public StringTypealizRClassModel Build()
	{
		var nested = nestedTypes
			.Values
			.Select(x => x.Build())
			.ToArray()
		;

		return new(name, markerType, rootNamespace, members, nested);
    }

	
}
