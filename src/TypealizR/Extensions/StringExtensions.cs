using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypealizR.Extensions;

namespace System;
public static class StringExtensions
{
    internal static string ToMultiline(this IEnumerable<string> that, string prependLinesWith = "", bool appendNewLineAfterEach = true)
    {
        if (that is null)
        {
            throw new ArgumentNullException(nameof(that));
        }

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

    internal static string ToCommaDelimited(this IEnumerable<string> that) => that.Join(", ");

    internal static string ToSpaceDelimited(this IEnumerable<string> that) => that.Join(" ");

    public static string Join(this IEnumerable<string> that, string seperator = "") => string.Join(seperator, that);

    internal static string RemoveAndReplaceDuplicatesOf(this string s, string seperator, string join) => string.Join(join, s.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries));

    internal static string ReplaceInvalidForMemberNameWith(this string that, char replacement) => new(that.Select(x => x.IsValidInIdentifier(false) ? x : replacement).ToArray());

    internal static string Escape(this string that) => that?.Replace("\"", "\"\"") ?? "";
}
