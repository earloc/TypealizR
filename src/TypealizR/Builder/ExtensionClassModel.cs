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
    public string Visibility => Target.Visibility.ToString().ToLower();
	public string TypeName => $"IStringLocalizerExtensions_{Target.FullNameForClassName}";

	public IEnumerable<ExtensionMethodModel> Methods { get; }

    public readonly TypeModel Target;

    private readonly HashSet<string> usings = new()
    {
        "System.CodeDom.Compiler",
        "System.Diagnostics",
        "System.Diagnostics.CodeAnalysis"
    };

    public IEnumerable<Diagnostic> Diagnostics { get; }


	public ExtensionClassModel(TypeModel target, string rootNamespace, IEnumerable<ExtensionMethodModel> methods, IEnumerable<Diagnostic> diagnostics)
    {
		Target = target;
		Methods = methods;
		Diagnostics = diagnostics;

        usings.Add(rootNamespace);
		usings.Add(target.Namespace);
    }

	public string FileName => $"IStringLocalizerExtensions_{Target.FullName}.g.cs";

	public string ToCSharp(Type generatorType) => $$"""
        // <auto-generated/>
        {{Usings.Select(x => $"using {x};").ToMultiline()}}
        namespace Microsoft.Extensions.Localization
        {

            {{generatorType.GeneratedCodeAttribute()}}
            {{Visibility}} static partial class {{TypeName}}
            {

                {{Methods.Select(x => x.ToCSharp()).ToMultiline()}}

            }
        }
        """;
}