using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;
using TypealizR.Extensibility;
using TypealizR.Builder;
using System;
using TypealizR.Core;

namespace TypealizR;

[Generator]
public sealed class StringFormatterSourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((x, cancel) => GeneratorOptions.From(x.GlobalOptions));
		var stringFormatterExistsProvider = context.CompilationProvider.Select((x, cancel) => !x.ContainsSymbolsWithName(StringFormatterClassBuilder.TypeName, SymbolFilter.Type));
		
		context.RegisterSourceOutput(optionsProvider
			.Combine(stringFormatterExistsProvider),
			(ctxt, source) =>
			{
				var stringFormatterExists = source.Right;
				var options = source.Left;

				AddStringFormatterClass(ctxt, options, stringFormatterExists);
			});
	}

	private void AddStringFormatterClass(SourceProductionContext ctxt, GeneratorOptions options, bool stringFormatterExists)
	{
		var stringFormatterBuilder = new StringFormatterClassBuilder(options.RootNamespace);
		if (stringFormatterExists)
		{
			stringFormatterBuilder.UserModeImplementationIsProvided();
		}

		ctxt.AddSource($"{StringFormatterClassBuilder.TypeName}.g.cs", stringFormatterBuilder.Build(GetType()));
	}
}
