using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class TypeInfo
{
	public TypeInfo(string @namespace, string name)
	{
		Namespace = @namespace;
		Name = name;
	}

    public string Namespace { get; }
    public string Name { get; }

	public string FullName => $"{Namespace}.{Name}";

}
internal class StringLocalizerExtensionClassBuilder
{
	private List<StringLocalizerExtensionMethodBuilder> methodBuilders = new();

	public StringLocalizerExtensionClassBuilder WithMethodFor(string key, string value)
	{
        methodBuilders.Add(new(key, value));
		return this;
	}

	public ExtensionClassInfo Build(TypeInfo target)
	{
		var methods = methodBuilders
			.Select(x => x.Build(target))
			.ToArray()
		;

		return new(target, methods);

    }

	public IEnumerable<StringLocalizerExtensionMethodBuilder> Methods => methodBuilders;

}
