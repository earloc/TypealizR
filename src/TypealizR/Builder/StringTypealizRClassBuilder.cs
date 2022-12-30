using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Diagnostics;
using TypealizR.Values;

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

    private readonly DeduplicatingCollection<InstanceMemberModel> members = new();

    public StringTypealizRClassBuilder WithMember(string key, string rawKey, string value, DiagnosticsCollector diagnostics)
    {
        var builder = new InstanceMemberBuilder(key, rawKey, value);
        var model = builder.Build(diagnostics);

        members.Add(model, diagnostics);

        return this;
    }

    private readonly Dictionary<string, StringTypealizRClassBuilder> nestedTypes = new();

    public StringTypealizRClassBuilder WithGroups(string key, string rawKey, string value, IEnumerable<MemberName> groups, DiagnosticsCollector diagnostics)
    {
        if (!groups.Any())
        {
            WithMember(key, rawKey, value, diagnostics);
            return this;
        }

        var firstLevel = groups.First();

        if (!nestedTypes.ContainsKey(firstLevel))
        {
            nestedTypes[firstLevel] = new StringTypealizRClassBuilder(markerType, $"{firstLevel}", rootNamespace, severityConfig);
        }

        nestedTypes[firstLevel].WithGroups(key, rawKey, value, groups.Skip(1), diagnostics);

        return this;
    }

    public StringTypealizRClassModel Build()
    {
        var nested = nestedTypes
            .Values
            .Select(x => x.Build())
            .ToArray()
        ;

        return new(name, markerType, rootNamespace, members.Items, nested);
    }

    
}
