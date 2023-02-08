using System;
using System.Collections.Generic;
using System.Linq;

namespace TypealizR;

internal class CodeFirstParameterModel
{

    public CodeFirstParameterModel(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public string Type { get; }
}

internal static class CodeFirstParameterModelExtensions
{
    public static string ToCharpDeclaration(this IEnumerable<CodeFirstParameterModel> that)
        => that.Select(x => $"{x.Type} {x.Name}").ToCommaDelimited();

    public static string ToCSharpInvocation(this IEnumerable<CodeFirstParameterModel> that)
        => that.Select(x => x.Name).ToCommaDelimited();
}
