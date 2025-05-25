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

    private string FallbackKeyName => $"{key}{_.FallBackKeySuffix}";
    private string KeyName => $"{key}{_.KeySuffix}";
    private string RawName => $"{key}{_.RawSuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-method
        {{moreSpaces}}        private const string {{KeyName}} = @"{{key}}";
        {{moreSpaces}}        private const string {{FallbackKeyName}} = @"{{fallbackKey}}";
        {{moreSpaces}}        public {{returnType}} {{RawName}}
        {{moreSpaces}}        {
        {{moreSpaces}}            get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer[{{KeyName}}];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer[{{FallbackKeyName}}];
        {{moreSpaces}}            }
        {{moreSpaces}}        }
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}){{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}            var localizedString = localizer[{{KeyName}}, {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}            if (!localizedString.ResourceNotFound)
        {{moreSpaces}}            {
        {{moreSpaces}}                return localizedString;
        {{moreSpaces}}            }
        {{moreSpaces}}          return localizer[{{FallbackKeyName}}, {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
