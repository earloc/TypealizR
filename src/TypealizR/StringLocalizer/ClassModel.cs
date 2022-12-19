using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace TypealizR.StringLocalizer;

internal class ClassModel
{
    public readonly TypeModel Target;

    private readonly HashSet<string> usings = new()
    {
        "System.CodeDom.Compiler",
        "System.Diagnostics",
        "System.Diagnostics.CodeAnalysis"
    };

    public IEnumerable<string> Usings => usings;
	public IEnumerable<MethodModel> Methods { get; }
    public IEnumerable<Diagnostic> Diagnostics { get; }

    public string Visibility => Target.Visibility.ToString().ToLower();

	public ClassModel(TypeModel target, string rootNamespace, IEnumerable<MethodModel> methods, IEnumerable<Diagnostic> warningsAndErrors)
    {
		Target = target;
		Methods = methods;
		Diagnostics = warningsAndErrors;

        usings.Add(rootNamespace);
		usings.Add(target.Namespace);

	}

}