using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;
internal static class StringExtensions
{
	public static string ToMultiline(this IEnumerable<string> that)
	{
		var builder = new StringBuilder();

		foreach (var line in that)
		{
			builder.AppendLine(line);
			builder.AppendLine();
		}

		return builder.ToString().Trim();
	}

	public static string ToCommaDelimited(this IEnumerable<string> that) => string.Join(", ", that);

	public static IEnumerable<string> TrimWrap(this string that) => that
		.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
		.Select(x => x?.Trim() ?? "")
		.Where(x => !string.IsNullOrEmpty(x))
		.ToArray();


}
