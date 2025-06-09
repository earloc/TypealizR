namespace TypealizR.CodeFirst;

internal sealed class CodeFirstParameterBuilder
{
    private readonly string name;
    private readonly string type;

    public CodeFirstParameterBuilder(string name, string type)
    {
        this.name = name;
        this.type = type;
    }

    internal CodeFirstParameterModel Build() => new(name, type);
}
