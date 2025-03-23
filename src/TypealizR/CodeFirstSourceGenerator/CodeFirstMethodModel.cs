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

    private string FallbackKeyName => $"{key}{_.FallBackKeySuffix}";
    private string KeyName => $"{key}{_.KeySuffix}";
    private string RawName => $"{key}{_.RawSuffix}";

    internal string ToCSharp(string moreSpaces = "") => $$"""

        {{moreSpaces}}        #region {{key}}-method
        {{moreSpaces}}        private const string {{KeyName}} = @"{{key}}";
        {{moreSpaces}}        private const string {{FallbackKeyName}} = @"{{fallbackKey}}";
        {{moreSpaces}}        public {{returnType}} {{RawName}} => localizer[{{KeyName}}].Or(localizer[{{FallbackKeyName}}]);
        {{moreSpaces}}        public {{returnType}} {{key}} ({{parameters.ToCharpDeclaration()}}) => localizer[{{KeyName}}, {{parameters.ToCSharpInvocation()}}].Or(localizer[{{FallbackKeyName}}, {{parameters.ToCSharpInvocation()}}]);
        {{moreSpaces}}        #endregion

        """;
}
