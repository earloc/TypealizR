﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using TypealizR.Extensions;

namespace TypealizR.Core;
internal class MemberName
{
    //refactor me
    private readonly string name;
    private string? nameOverride;
    internal static readonly char[] separator = new[] { ' ' };

    public MemberName(string raw)
    {
        var value = new string(raw.SkipWhile(x => !x.IsValidInIdentifier(true)).ToArray());

        value = value.RemoveAndReplaceDuplicatesOf(" ", "@");
        value = value.Replace(".", "");

        value = new string(
            value
                .Trim('_')
                .Select((x, i) => x.IsValidInIdentifier(i == 0) ? x : ' ')
                .ToArray()
        );

        value = string.Join(" ",
                value.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrEmpty(x))
                )
                .Replace("___", "__")
                .ReplaceInvalidForMemberNameWith('_');

        name = value.Trim('_');
    }

    public static implicit operator string(MemberName that) => that.nameOverride ?? that.name;
    public override string ToString() => nameOverride ?? name;

    internal bool IsValidMethodName()
    {
        var method = SourceText.From($$"""

            namespace ProbingSpace;

            public class Programm {
                static void main() {
                }

                void {{name}} () {}
            }
""");
        var syntax = CSharpSyntaxTree.ParseText(method);
        var compilation = CSharpCompilation.Create("probe").AddSyntaxTrees(syntax);
        var diagnostics = compilation.GetDiagnostics();

        var error = diagnostics.FirstOrDefault(x => x.Id == "CS1519");

        return error == null;
    }

    internal void MakeCompilable() => nameOverride = $"_{name}";
}
