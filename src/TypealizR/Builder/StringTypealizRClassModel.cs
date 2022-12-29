﻿using System;using System.Collections.Generic;using System.Linq;using Microsoft.CodeAnalysis;using TypealizR.Extensions;namespace TypealizR.Builder;internal class StringTypealizRClassModel{    public IEnumerable<string> Usings => usings;    private readonly string typeName;    private readonly TypeModel markerType;    private readonly IEnumerable<InstanceMemberModel> members;    private readonly IEnumerable<StringTypealizRClassModel> groups;    private readonly HashSet<string> usings = new()    {        "System.CodeDom.Compiler",        "System.Collections.Generic",        "System.Diagnostics",        "System.Diagnostics.CodeAnalysis",        "Microsoft.Extensions.Localization",        "TypealizR.Abstractions"    };    public StringTypealizRClassModel(string typeName, TypeModel markerType, string rootNamespace, IEnumerable<InstanceMemberModel> members, IEnumerable<StringTypealizRClassModel> groups)    {        this.typeName = typeName;        this.markerType = markerType;        this.members = members;        usings.Add(rootNamespace);        this.groups = groups;    }    public string FileName => $"StringTypealizR_{markerType.FullName}.g.cs";    public string ToNestedCsharp(Type generatorType, string indent) => $$"""        {{indent}}{{generatorType.GeneratedCodeAttribute()}}        {{indent}}{{markerType.Visibility.ToString().ToLower()}} partial class {{typeName}}Group        {{indent}}{                {{indent}}    private readonly IStringLocalizer<{{markerType.FullName}}> localizer;                {{indent}}    [DebuggerStepThrough]        {{indent}}    public {{typeName}}Group(IStringLocalizer<{{markerType.FullName}}> localizer)        {{indent}}    {        {{indent}}        this.localizer = localizer;        {{groups.Select(x => $"{indent}        {x.typeName} = new {x.typeName}Group(localizer);").ToMultiline(appendNewLineAfterEach: false)}}        {{indent}}    }                {{indent}}    {{members.Select(x => x.ToCSharp()).ToMultiline($"    ")}}        {{indent}}              {{groups.Select(x => $"{indent}    public {x.typeName}Group {x.typeName} {{ get; }}").ToMultiline(appendNewLineAfterEach: false)}}        {{groups.Select(x => x.ToNestedCsharp(generatorType, $"{indent}    ")).ToMultiline(appendNewLineAfterEach: false)}}                {{indent}}}        """;    public string ToCSharp(Type generatorType) => $$"""        // <auto-generated/>        {{Usings.Select(x => $"using {x};").ToMultiline(appendNewLineAfterEach: false)}}        namespace {{markerType.Namespace}}.TypealizR        {            {{generatorType.GeneratedCodeAttribute()}}            {{markerType.Visibility.ToString().ToLower()}} partial class {{typeName}}            {                        private readonly IStringLocalizer<{{markerType.FullName}}> localizer;                        public {{typeName}}(IStringLocalizer<{{markerType.FullName}}> localizer)                {                    this.localizer = localizer;        {{groups.Select(x => $"            {x.typeName} = new {x.typeName}Group(localizer);").ToMultiline(appendNewLineAfterEach: false)}}                }                {{members.Select(x => x.ToCSharp()).ToMultiline("        ")}}                {{groups.Select(x => $"        public {x.typeName}Group {x.typeName} {{ get; }}").ToMultiline(appendNewLineAfterEach: false)}}                {{groups.Select(x => x.ToNestedCsharp(generatorType, "        ")).ToMultiline(appendNewLineAfterEach: false)}}                /// <summary>                /// Gets the string resource with the given name.                /// </summary>                /// <param name="name">The name of the string resource.</param>                /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>                public LocalizedString this[string name] => this.localizer[name];                /// <summary>                /// Gets the string resource with the given name and formatted with the supplied arguments.                /// </summary>                /// <param name="name">The name of the string resource.</param>                /// <param name="arguments">The values to format the string with.</param>                /// <returns>The formatted string resource as a <see cref="LocalizedString"/>.</returns>                public LocalizedString this[string name, params object[] arguments] => this.localizer[name, arguments];                /// <summary>                /// Gets all string resources.                /// </summary>                /// <param name="includeParentCultures">                /// A <see cref="System.Boolean"/> indicating whether to include strings from parent cultures.                /// </param>                /// <returns>The strings.</returns>                public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => this.localizer.GetAllStrings(includeParentCultures);                public IStringLocalizer<{{markerType.FullName}}> Localizer => this.localizer;            }        }        """;}