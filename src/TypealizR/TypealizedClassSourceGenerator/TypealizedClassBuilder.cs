using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;
internal partial class TypealizedClassBuilder
{    private readonly bool useParametersInMethodNames;    private readonly TypeModel markerType;
    private readonly string name;
    private readonly string rootNamespace;
    private readonly IDictionary<string, DiagnosticSeverity> severityConfig;

    public TypealizedClassBuilder(bool useParametersInMethodNames, TypeModel markerType, string name, string rootNamespace, IDictionary<string, DiagnosticSeverity> severityConfig)
    {        this.useParametersInMethodNames = useParametersInMethodNames;        this.markerType = markerType;
        this.name = name;
        this.rootNamespace = rootNamespace;
        this.severityConfig = severityConfig;
    }

    private readonly DeduplicatingCollection<InstanceMemberModel> members = new();

    public TypealizedClassBuilder WithMember(string key, string rawKey, string value, DiagnosticsCollector diagnostics)
    {
        var builder = new InstanceMemberBuilder(useParametersInMethodNames, key, rawKey, value);
        var model = builder.Build(diagnostics);

        members.Add(model, diagnostics);

        return this;
    }

    private readonly Dictionary<string, TypealizedClassBuilder> nestedTypes = [];

    public TypealizedClassBuilder WithGroups(string key, string rawKey, string value, IEnumerable<MemberName> groups, DiagnosticsCollector diagnostics)
    {
        var firstLevel = groups.FirstOrDefault();
        if (firstLevel is null)
        {
            WithMember(key, rawKey, value, diagnostics);
            return this;
        }

        if (!nestedTypes.ContainsKey(firstLevel))
        {
            nestedTypes[firstLevel] = new TypealizedClassBuilder(useParametersInMethodNames, markerType, $"{firstLevel}", rootNamespace, severityConfig);
        }

        nestedTypes[firstLevel].WithGroups(key, rawKey, value, groups.Skip(1), diagnostics);

        return this;
    }

    public TypealizedClassModel Build()
    {
        var nested = nestedTypes
            .Values
            .Select(x => x.Build())
            .ToArray()
        ;

        return new(name, markerType, rootNamespace, members.Items, nested);
    }


}
