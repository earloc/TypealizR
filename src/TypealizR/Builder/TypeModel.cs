namespace TypealizR.Builder;
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
	public string FullNameForClassName => FullName.Replace(".", "_");


}
