namespace TypealizR.StringLocalizer;

internal class TypeModel
{
	public TypeModel(string @namespace, string name, Visibility visibility = Visibility.Internal)
	{
		Namespace = @namespace;
		Name = name;
		Visibility = visibility;
	}

    public string Namespace { get; }
    public string Name { get; }

	public Visibility Visibility { get; }

	public string FullName => $"{Namespace}.{Name}";
	public string FullNameForExtensionsClass => FullName.Replace(".", "_");


}
