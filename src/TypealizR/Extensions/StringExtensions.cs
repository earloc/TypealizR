using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;
internal static class StringExtensions
{
	public static string ToMultiline(this IEnumerable<string> that) => string.Join("\r", that);

	public static string ToCommaDelimited(this IEnumerable<string> that) => string.Join(", ", that);

	public static IEnumerable<string> TrimWrap(this string that) => that
		.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
		.Select(x => x?.Trim() ?? "")
		.Where(x => !string.IsNullOrEmpty(x))
		.ToArray();


}
