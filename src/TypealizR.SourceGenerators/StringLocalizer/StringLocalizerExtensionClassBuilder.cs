using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class StringLocalizerExtensionClassBuilder
{
	private List<StringLocalizerExtensionMethodBuilder> methodBuilders = new();

	public StringLocalizerExtensionClassBuilder WithMethodFor(string key, string value)
	{
        methodBuilders.Add(new(key, value));
		return this;
	}

	public ExtensionClassInfo Build(string targetNamespace, string targetTypeName)
	{
		var methods = methodBuilders
			.Select(x => x.Build(targetNamespace, targetTypeName))
			.ToArray()
		;

		return new(targetNamespace, targetTypeName, methods);

    }

	public IEnumerable<StringLocalizerExtensionMethodBuilder> Methods => methodBuilders;

}
