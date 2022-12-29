﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Microsoft.CodeAnalysis;
using TypealizR.Extensions;

namespace TypealizR.Builder;
internal class ExtensionClassModel
{
    public IEnumerable<string> Usings => usings;

	private readonly TypeModel markertType;
	private readonly IEnumerable<ExtensionMethodModel> methods;

	public ExtensionClassModel(TypeModel markertType, string rootNamespace, IEnumerable<ExtensionMethodModel> methods)
    {
		this.markertType = markertType;
		this.methods = methods;
		usings.Add(rootNamespace);
		usings.Add($"{rootNamespace}.TypealizR");
		usings.Add(markertType.Namespace);
	}

	private readonly HashSet<string> usings = new()
	{
		"System.CodeDom.Compiler",
		"System.Diagnostics",
		"System.Diagnostics.CodeAnalysis"
	};

	public string FileName => $"IStringLocalizerExtensions_{markertType.FullName}.g.cs";

	public string ToCSharp(Type generatorType) => $$"""
        // <auto-generated/>
        {{Usings.Select(x => $"using {x};").ToMultiline(appendNewLineAfterEach: false)}}
        namespace Microsoft.Extensions.Localization
        {

            {{generatorType.GeneratedCodeAttribute()}}
            {{markertType.Visibility.ToString().ToLower()}} static partial class IStringLocalizerExtensions_{{markertType.FullNameForClassName}}
            {

                {{methods.Select(x => x.ToCSharp()).ToMultiline()}}

            }
        }

        """;
}