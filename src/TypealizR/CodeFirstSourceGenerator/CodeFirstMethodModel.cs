using System;
using System.Linq;

namespace TypealizR;

internal class CodeFirstMethodModel
{
    private readonly string key;
    private readonly string fallbackKey;
    private readonly CodeFirstParameterModel[] parameters;
    private readonly string returnType;

    public CodeFirstMethodModel(string key, CodeFirstParameterModel[] parameters, string returnType, string? fallbackKey)
    {
        this.key = key;
        this.parameters = parameters;
        this.returnType = returnType;
        this.fallbackKey = fallbackKey ?? $"{key} {parameters.Select((x, i) => $$"""{{{i}}}""").ToSpaceDelimited()}";
    }

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-method

        {{moreSpaces}}        public {{returnType}} {{key}}Raw => localizer["{{key}}"].Or(localizer["{{fallbackKey}}"]);
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}) => localizer["{{key}}"}, {{parameters.ToCSharpInvocation()}}].Or(localizer["{{fallbackKey}}", {{parameters.ToCSharpInvocation()}}]);
        
        {{moreSpaces}}        #endregion

        """;
}
