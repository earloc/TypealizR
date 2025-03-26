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
    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-property

        {{moreSpaces}}        public {{returnType}} {{key}} => localizer["{{key}}"].Or(localizer["{{fallbackKey}}"]);

        {{moreSpaces}}        #endregion

        """;
}
