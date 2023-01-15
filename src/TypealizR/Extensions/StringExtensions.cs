using System.Collections.Generic;
using System.Text;

namespace System;
internal static class StringExtensions
{
	public static string ToMultiline(this IEnumerable<string> that, string prependLinesWith = "", bool appendNewLineAfterEach = true)
	{
		var builder = new StringBuilder();

		int i = 0;
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

	public static string ToCommaDelimited(this IEnumerable<string> that) 
        => string.Join(", ", that);

    public static string RemoveAndReplaceDuplicatesOf(this string s, string seperator, string join) 
        => string.Join(join, s.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries));
}
