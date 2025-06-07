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

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region typealized {{key}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{fallbackKey}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        public {{returnType}} {{key}}{{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}          get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer[@"{{key}}"];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer[@"{{fallbackKey}}"];
        {{moreSpaces}}          }
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
