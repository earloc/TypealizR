using Microsoft.CodeAnalysis;

namespace TypealizR.Core;
public class TypeModel(string @namespace, string name, Accessibility accessibility = Accessibility.Internal)
{
    public string Namespace { get; } = @namespace;
    public string Name { get; } = name;

    public Accessibility Accessibility { get; } = accessibility;

    public string FullName => $"{Namespace}.{Name}";
    public string GlobalFullName => $"global::{FullName}";

    public string FullNameForClassName => FullName.Replace(".", "");


}
