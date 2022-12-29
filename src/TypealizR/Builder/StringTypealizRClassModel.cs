﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Extensions;

namespace TypealizR.Builder;
internal class StringTypealizRClassModel
{
    public IEnumerable<string> Usings => usings;

	private readonly string typeName;
	private readonly TypeModel markerType;
	private readonly IEnumerable<InstanceMemberModel> members;
	private readonly IEnumerable<StringTypealizRClassModel> groups;
	private readonly HashSet<string> usings = new()
    {
		"TypealizR.Abstractions"
	};

	public StringTypealizRClassModel(string typeName, TypeModel markerType, string rootNamespace, IEnumerable<InstanceMemberModel> members, IEnumerable<StringTypealizRClassModel> groups)
    {
		this.typeName = typeName;
		this.markerType = markerType;
		this.members = members;
		usings.Add(rootNamespace);
		this.groups = groups;
    }

	public string FileName => $"StringTypealizR_{markerType.FullName}.g.cs";


    public string ToNestedCsharp(Type generatorType) => $$"""

        {{generatorType.GeneratedCodeAttribute()}}
        {{markerType.Visibility.ToString().ToLower()}} partial class {{typeName}}Group
        {

            private readonly IStringLocalizer<{{markerType.Name}}> localizer;
            [DebuggerStepThrough]
            public {{typeName}}Group(IStringLocalizer<{{markerType.Name}}> localizer)
            {
                this.localizer = localizer;
                {{groups.Select(x => $"{x.typeName} = new {x.typeName}Group(localizer);").ToMultiline("            ", appendNewLineAfterEach: false)}}
            }

            {{members.Select(x => x.ToCSharp()).ToMultiline()}}
    
            {{groups.Select(x => $"public {x.typeName}Group {x.typeName} {{ get; }}").ToMultiline("        ", appendNewLineAfterEach: false)}}

            {{groups.Select(x => x.ToNestedCsharp(generatorType)).ToMultiline("        ", appendNewLineAfterEach: false)}}
        }
    """;


	public string ToCSharp(Type generatorType) => $$"""
        // <auto-generated/>
        {{Usings.Select(x => $"using {x};").ToMultiline(appendNewLineAfterEach: false)}}
        namespace {{markerType.Namespace}}.TypealizR
        {

            {{generatorType.GeneratedCodeAttribute()}}
            {{markerType.Visibility.ToString().ToLower()}} partial class {{typeName}} : IStringTypealizR<{{markerType.Name}}>
            {

                private readonly IStringLocalizer<{{markerType.Name}}> localizer;
                public {{typeName}}(IStringLocalizer<{{markerType.Name}}> localizer)
                {
                    this.localizer = localizer;
                    {{groups.Select(x => $"{x.typeName} = new {x.typeName}Group(localizer);").ToMultiline("            ", appendNewLineAfterEach: false)}}
                }

                {{members.Select(x => x.ToCSharp()).ToMultiline()}}

                {{groups.Select(x => $"public {x.typeName}Group {x.typeName} {{ get; }}").ToMultiline("        ", appendNewLineAfterEach: false)}}

            }

            {{groups.Select(x => x.ToNestedCsharp(generatorType)).ToMultiline("        ", appendNewLineAfterEach: false)}}
        }

        """;
}