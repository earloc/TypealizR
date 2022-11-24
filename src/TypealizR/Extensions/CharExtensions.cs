using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TypealizR.Extensions;
internal static class CharExtensions
{
    //thx https://stackoverflow.com/a/60820647/4136104
    internal static bool IsValidInIdentifier(this char c, bool firstChar = true)
    {
        switch (char.GetUnicodeCategory(c))
        {
            case UnicodeCategory.UppercaseLetter:
            case UnicodeCategory.LowercaseLetter:
            case UnicodeCategory.TitlecaseLetter:
            case UnicodeCategory.ModifierLetter:
            case UnicodeCategory.OtherLetter:
                // Always allowed in C# identifiers
                return true;

            case UnicodeCategory.LetterNumber:
            case UnicodeCategory.NonSpacingMark:
            case UnicodeCategory.SpacingCombiningMark:
            case UnicodeCategory.DecimalDigitNumber:
            case UnicodeCategory.ConnectorPunctuation:
            case UnicodeCategory.Format:
                // Only allowed after first char
                return !firstChar;
            default:
                return false;
        }
    }
}
