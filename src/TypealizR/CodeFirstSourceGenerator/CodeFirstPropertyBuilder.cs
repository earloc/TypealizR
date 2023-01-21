using System;

namespace TypealizR;

internal class CodeFirstPropertyBuilder
{
    private readonly string name;
    private readonly string defaultValue;

    public CodeFirstPropertyBuilder(string name, string? defaultValue)
    {
        this.name = name;
        this.defaultValue = defaultValue ?? name;
    }

    internal CodeFirstPropertyModel Build()
    {
        return new CodeFirstPropertyModel(name, "LocalizedString", defaultValue);
    }
}
