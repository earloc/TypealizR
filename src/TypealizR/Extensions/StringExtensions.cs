using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypealizR.Extensions;

namespace System;
internal static class StringExtensions
{
    public static string ToMultiline(this IEnumerable<string> that, string prependLinesWith = "", bool appendNewLineAfterEach = true)
    {
        var builder = new StringBuilder();

        var i = 0;
        foreach (var line in that)
        {
            if (i++ > 0)
            {
                builder.Append(prependLinesWith);
            }
            builder.AppendLine(line);
            if (appendNewLineAfterEach)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }

    public static string ToCommaDelimited(this IEnumerable<string> that) => that.Join(", ");

    public static string ToSpaceDelimited(this IEnumerable<string> that) => that.Join(" ");

    public static string Join(this IEnumerable<string> that, string seperator = "") => string.Join(seperator, that);

    public static string RemoveAndReplaceDuplicatesOf(this string s, string seperator, string join) => string.Join(join, s.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries));

    public static string ReplaceInvalidForMemberNameWith(this string that, char replacement) => new(that.Select(x => x.IsValidInIdentifier(false) ? x : replacement).ToArray());

    public static string Escape(this string that) => that.Replace("\"", "\"\"");
}
