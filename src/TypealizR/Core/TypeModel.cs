using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Core;
public class TypeModel(string @namespace, string name, string[] containingTypeNames, Accessibility accessibility)
{
    public string Namespace { get; } = @namespace;
    public string Name { get; } = name;

    public Accessibility Accessibility { get; } = accessibility;

    public string FullName => $"{Namespace}.{ContainingTypes}{Name}";

    internal string ContainingTypes => containingTypeNames.Length <= 0
        ? string.Empty
        : containingTypeNames.Join("+") + "+"
    ;

    public string GlobalFullName => $"global::{FullName}";

    public string FullNameForClassName => FullName.Replace(".", "");


}
