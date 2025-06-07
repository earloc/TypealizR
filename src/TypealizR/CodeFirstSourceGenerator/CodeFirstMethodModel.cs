using System;
using System.Linq;

namespace TypealizR;

internal class CodeFirstMethodModel
{
    private readonly string key;
    private readonly string escapedKey;
    private readonly string escapedFallbackKey;

    private readonly string? remarksComment;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel(string key, CodeFirstParameterModel[] parameters, string returnType, string? fallbackKey, string? remarks)
    {
        this.key = key;
        escapedKey = $@"""{key}""";

        this.parameters = parameters;
        this.returnType = returnType;
        fallbackKey = fallbackKey ?? $"{key} {parameters.Select((x, i) => $$"""{{{i}}}""").ToSpaceDelimited()}";
        escapedFallbackKey = $@"""{fallbackKey}""";

        this.remarksComment = string.IsNullOrEmpty(remarks)? "" : $" // {remarks}";
    }

    private string RawName => $"{key}{_.RawSuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region typealized {{key}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{escapedFallbackKey}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        public {{returnType}} {{RawName}}
        {{moreSpaces}}        {
        {{moreSpaces}}            get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer[{{escapedKey}}];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer[$""{{escapedFallbackKey}}""];
        {{moreSpaces}}            }
        {{moreSpaces}}        }
        {{moreSpaces}}        
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{escapedFallbackKey}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}){{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}            var localizedString = localizer[@"{{key}}", {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}            if (!localizedString.ResourceNotFound)
        {{moreSpaces}}            {
        {{moreSpaces}}                return localizedString;
        {{moreSpaces}}            }
        {{moreSpaces}}          return localizer[$""{{escapedFallbackKey}}"", {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
