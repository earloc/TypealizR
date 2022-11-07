using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class StringLocalizerExtensionClassBuilder
{
	private List<StringLocalizerExtensionMethodBuilder> methodBuilders = new();

	public StringLocalizerExtensionClassBuilder(string projectDirectoryPath, string projectRootNamespace)
	{
	}

	public StringLocalizerExtensionClassBuilder WithMethodFor(string key, string value)
	{
        methodBuilders.Add(new(key, value));
		return this;
	}

	public ExtensionClassInfo Build(string targetTypeName, string targetNamespace)
	{
		var methods = methodBuilders
			.Select(x => x.Build(targetTypeName))
			.ToArray()
		;

		return new(targetTypeName, targetNamespace, methods);

    }

}
