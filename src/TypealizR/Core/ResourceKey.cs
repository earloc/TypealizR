namespace TypealizR.Core;

internal class ResourceKey
{
    public string Value { get; }
    private string sanitizedValue;

    internal ResourceKey(string value)
    {
        Value = value;
        sanitizedValue = value.Replace("\"", "\\\"");
    }

    public override string ToString() => sanitizedValue;
}