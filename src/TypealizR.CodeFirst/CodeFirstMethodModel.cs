using System.Linq;

namespace TypealizR.CodeFirst;

internal class CodeFirstMethodModel
{
    private readonly string key;
    private readonly string escapedKey;
    private readonly string fallbackKey;
    private readonly string escapedFallbackKey;

    private readonly string? remarksComment;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel(string key, CodeFirstParameterModel[] parameters, string returnType, string? fallbackKey, string? remarks)
    {
        this.key = key;
        escapedKey = $@"""{key}""";
        this.fallbackKey = fallbackKey ?? $"{key} {parameters.Select((x, i) => $$"""{{{i}}}""").ToSpaceDelimited()}".Trim();

        escapedFallbackKey = $$""""
            @"{{this.fallbackKey.Trim()}}"
            """".Trim();

        this.parameters = parameters;
        this.returnType = returnType;

        this.remarksComment = string.IsNullOrEmpty(remarks)? "" : $" // {remarks}";
    }

    private string RawName => $"{key}{_.RawSuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region typealized {{key}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{fallbackKey.Split('\n').ToMultiline($"{moreSpaces}        /// ", true, true)}}
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
        {{moreSpaces}}              return localizer[{{escapedFallbackKey}}];
        {{moreSpaces}}            }
        {{moreSpaces}}        }
        {{moreSpaces}}        
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{fallbackKey.Split('\n').ToMultiline($"{moreSpaces}        /// ", true, true)}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}){{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}            var localizedString = localizer[@"{{key}}", {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}            if (!localizedString.ResourceNotFound)
        {{moreSpaces}}            {
        {{moreSpaces}}                return localizedString;
        {{moreSpaces}}            }
        {{moreSpaces}}          return localizer[{{escapedFallbackKey}}, {{parameters.ToCSharpInvocation()}}];
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
