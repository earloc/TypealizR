namespace TypealizR;

internal class CodeFirstPropertyModel
{
    private readonly string key;
    private readonly string returnType;
    private readonly string fallbackKey;

    public CodeFirstPropertyModel(string key, string returnType, string fallbackKey)
    {
        this.key = key;
        this.returnType = returnType;
        this.fallbackKey = fallbackKey;
    }
    private string FallbackKeyName => $"{key}{_.FallBackKeySuffix}";
    private string KeyName => $"{key}{_.KeySuffix}";


    internal string ToCSharp() => $$"""
        private const string {{KeyName}} = @"{{key}}";
                private const string {{FallbackKeyName}} = @"{{fallbackKey}}";
                public {{returnType}} {{key}} => localizer[{{KeyName}}].Or(localizer[{{FallbackKeyName}}]);
        """;
}
