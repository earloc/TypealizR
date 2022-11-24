namespace TypealizR.SourceGenerators.StringLocalizer;

internal class TypeModel
{
	public TypeModel(string @namespace, string name)
	{
		Namespace = @namespace;
		Name = name;
	}

    public string Namespace { get; }
    public string Name { get; }

	public string FullName => $"{Namespace}.{Name}";

}
