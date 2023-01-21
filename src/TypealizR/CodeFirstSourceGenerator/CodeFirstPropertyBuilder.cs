using System;

namespace TypealizR;

internal class CodeFirstPropertyBuilder
{
    private readonly string name;

    public CodeFirstPropertyBuilder(string name)
    {
        this.name = name;
    }

    internal CodeFirstPropertyModel Build()
    {
        return new CodeFirstPropertyModel(name, "LocalizedString", name);
    }
}
