using System;
using System.Globalization;
using System.Text;

namespace TypealizR.Extensions;

public static class SystemExtensions
{
    public static string GeneratedCodeAttribute(this Type that) => $@"[GeneratedCode(""{that.FullName}"", ""{that.Assembly.GetName().Version}"")]";

    public static string ToMultiline(this IEnumerable<string> that, string prependLinesWith = "", bool appendNewLineAfterEach = true, bool trimEach = false)
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
            builder.AppendLine(trimEach ? line.Trim() : line);
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

    public static bool IsValidInIdentifier(this char c, bool firstChar = true) => char.GetUnicodeCategory(c) switch
    {
        //thx https://stackoverflow.com/a/60820647/4136104
        UnicodeCategory.UppercaseLetter or
        UnicodeCategory.LowercaseLetter or
        UnicodeCategory.TitlecaseLetter or
        UnicodeCategory.ModifierLetter or
        UnicodeCategory.OtherLetter => true,// Always allowed in C# identifiers

        UnicodeCategory.LetterNumber or
        UnicodeCategory.NonSpacingMark or
        UnicodeCategory.SpacingCombiningMark or
        UnicodeCategory.DecimalDigitNumber or
        UnicodeCategory.ConnectorPunctuation or
        UnicodeCategory.Format => !firstChar,// Only allowed after first char
        _ => false,
    };
}
