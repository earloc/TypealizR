namespace TypealizR;

internal class CodeFirstPropertyModel
{
    private readonly string key;
    private readonly string returnType;
    private readonly string fallbackKey;
    private readonly string remarksComment;

    public CodeFirstPropertyModel(string key, string returnType, string fallbackKey, string? remarks)
    {
        this.key = key;
        this.returnType = returnType;
        this.fallbackKey = fallbackKey;
        this.remarksComment = string.IsNullOrEmpty(remarks) ? "" : $" // {remarks}";
    }
    private string FallbackKeyName => $"{key}{_.FallBackKeySuffix}";
    private string KeyName => $"{key}{_.KeySuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-property
        {{moreSpaces}}        private const string {{KeyName}} = @"{{key}}";
        {{moreSpaces}}        private const string {{FallbackKeyName}} = @"{{fallbackKey}}";
        {{moreSpaces}}        public {{returnType}} {{key}}{{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}          get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer[{{KeyName}}];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer[{{FallbackKeyName}}];
        {{moreSpaces}}          }
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
