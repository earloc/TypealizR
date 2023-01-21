using System;

namespace TypealizR;

internal class CodeFirstParameterBuilder
{
    private string name;
    private string type;

    public CodeFirstParameterBuilder(string name, string type)
    {
        this.name = name;
        this.type = type;
    }

    internal CodeFirstParameterModel Build() => new(name, type);
}
