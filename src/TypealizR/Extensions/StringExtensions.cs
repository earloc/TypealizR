using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;
internal static class StringExtensions
{
	public static string ToMultiline(this IEnumerable<string> that, bool appendNewLineAfterEach = true)
	{
		var builder = new StringBuilder();

		foreach (var line in that)
		{
			builder.AppendLine(line);
			if (appendNewLineAfterEach)
			{
				builder.AppendLine();
			}
		}

		return builder.ToString().Trim();
	}

	public static string ToCommaDelimited(this IEnumerable<string> that) => string.Join(", ", that);

}
