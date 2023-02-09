using System.Globalization;

namespace TypealizR.Extensions;
internal static class CharExtensions
{
    //thx https://stackoverflow.com/a/60820647/4136104
    internal static bool IsValidInIdentifier(this char c, bool firstChar = true)
    {
        return char.GetUnicodeCategory(c) switch
        {
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
}
