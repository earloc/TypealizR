using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TypealizR.StringLocalizer;

[assembly: InternalsVisibleTo("TypealizR.Tests")]

namespace TypealizR;

internal abstract class _
{
	private static readonly string generatorName = typeof(SourceGenerator).FullName;
	private static readonly Version generatorVersion = typeof(SourceGenerator).Assembly.GetName().Version;

	public static readonly string GeneratedCodeAttribute = $@"[GeneratedCode(""{generatorName}"", ""{generatorVersion}"")]";
}
