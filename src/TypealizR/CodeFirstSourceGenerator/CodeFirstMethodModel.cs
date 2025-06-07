using System;
using System.Linq;

namespace TypealizR;

internal class CodeFirstMethodModel
{
    private readonly string key;
    private readonly string fallbackKey;
    private readonly string? remarksComment;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel(string key, CodeFirstParameterModel[] parameters, string returnType, string? fallbackKey, string? remarks)
    {
        this.key = key;
        this.parameters = parameters;
        this.returnType = returnType;
        this.fallbackKey = fallbackKey ?? $"{key} {parameters.Select((x, i) => $$"""{{{i}}}""").ToSpaceDelimited()}";
        this.remarksComment = string.IsNullOrEmpty(remarks)? "" : $" // {remarks}";
    }

    private string RawName => $"{key}{_.RawSuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-method
        {{moreSpaces}}        public {{returnType}} {{RawName}}
        {{moreSpaces}}        {
        {{moreSpaces}}            get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer["{{key}}"];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer["{{fallbackKey}}"];
        {{moreSpaces}}            }
        {{moreSpaces}}        }
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}){{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}            var localizedString = localizer["{{key}}", {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}            if (!localizedString.ResourceNotFound)
        {{moreSpaces}}            {
        {{moreSpaces}}                return localizedString;
        {{moreSpaces}}            }
        {{moreSpaces}}          return localizer["{{fallbackKey}}", {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
