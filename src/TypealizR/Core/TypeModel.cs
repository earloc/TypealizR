using Microsoft.CodeAnalysis;

namespace TypealizR.Core;
public class TypeModel
{
    public TypeModel(string @namespace, string name, Accessibility accessibility = Accessibility.Internal)
    {
        Namespace = @namespace;
        Name = name;
        Accessibility = accessibility;
    }

    public string Namespace { get; }
    public string Name { get; }

    public Accessibility Accessibility { get; }

    public string FullName => $"{Namespace}.{Name}";
    public string GlobalFullName => $"global::{FullName}";

    public string FullNameForClassName => FullName.Replace(".", "");


}
