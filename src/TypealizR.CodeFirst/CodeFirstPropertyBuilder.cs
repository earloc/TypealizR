namespace TypealizR.CodeFirst;

internal sealed class CodeFirstPropertyBuilder
{
    private readonly string name;
    private readonly string defaultValue;
    private readonly string? remarks;

    public CodeFirstPropertyBuilder(string name, string? defaultValue, string? remarks)
    {
        this.name = name;
        this.defaultValue = defaultValue ?? name;
        this.remarks = remarks;
    }

    internal CodeFirstPropertyModel Build() => new(name, "LocalizedString", defaultValue, remarks);
}
