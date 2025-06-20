﻿using System;
using TypealizR.Extensions;

namespace TypealizR.CodeFirst;

internal sealed class CodeFirstPropertyModel
{
    private readonly string key;
    private readonly string escapedKey;
    private readonly string fallbackKey;
    private readonly string escapedFallbackKey;
    private readonly string returnType;
    private readonly string remarksComment;

    public CodeFirstPropertyModel(string key, string returnType, string fallbackKey, string? remarks)
    {
        this.key = key;
        this.fallbackKey = fallbackKey.Trim();

        escapedKey = $@"""{key}""";
        this.returnType = returnType;
        escapedFallbackKey = $$""""
            @"{{this.fallbackKey}}"
            """";

        this.remarksComment = string.IsNullOrEmpty(remarks) ? "" : $" // {remarks}";
    }

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region typealized {{key}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        /// {{fallbackKey.Split('\n').ToMultiline($"{moreSpaces}        /// ", true, true)}}
        {{moreSpaces}}        /// <summary>
        {{moreSpaces}}        public {{returnType}} {{key}}{{remarksComment}}
        {{moreSpaces}}        {
        {{moreSpaces}}          get
        {{moreSpaces}}            {
        {{moreSpaces}}              var localizedString = localizer[{{escapedKey}}];
        {{moreSpaces}}              if (!localizedString.ResourceNotFound)
        {{moreSpaces}}              {
        {{moreSpaces}}                  return localizedString;
        {{moreSpaces}}              }
        {{moreSpaces}}              return localizer[{{escapedFallbackKey}}];
        {{moreSpaces}}          }
        {{moreSpaces}}        }
        {{moreSpaces}}        #endregion

        """;
}
