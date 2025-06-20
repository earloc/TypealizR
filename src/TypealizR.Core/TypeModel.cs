using Microsoft.CodeAnalysis;
using TypealizR.Extensions;

namespace TypealizR.Core;
public class TypeModel(string @namespace, string name, string[] containingTypeNames, Accessibility accessibility)
{
    
    public string Namespace { get; } = @namespace;
    public string Name { get; } = name.Split([' '], StringSplitOptions.RemoveEmptyEntries).Join("_");

    public Accessibility Accessibility { get; } = accessibility;

    public string FullName => $"{Namespace}.{ContainingTypes}{Name}";

    internal string ContainingTypes => containingTypeNames.Length <= 0
        ? string.Empty
        :  containingTypeNames.Join("_") + "_"
    ;

    public string GlobalFullName => $"global::{FullName}";

    public string FullNameForClassName => FullName
        .Replace(".", "")
        .Replace("+", "_")
        .Split([' '], StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .Join("_")
    ;
}
