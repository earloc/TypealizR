using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Playground.Console;
internal static partial class TypealizR_StringFormatter
{
    internal static partial string Format(string s, object[] args) => string.Format(s, args);
    internal static partial string Extend(this string argument, string extension) => extension switch
    {
        "toUpper" => argument?.ToString().ToUpper() ?? argument,
        _ => argument
    };

    internal static partial double Extend(this double argument, string extension) => extension switch
    {
        "round0" => Math.Round(argument, 0),
        "round1" => Math.Round(argument, 1),
        "round2" => Math.Round(argument, 2),
        _ => argument
    };

    internal static partial int Extend(this int argument, string extension)
    {
        var extensionArgs = extension.Split(':');
        if (extensionArgs.Length != 2)
        {
            return argument;
        }

        var @operator = extensionArgs[0];

        if (!int.TryParse(extensionArgs[1], out var operand))
        {
            return argument;
        }

        return @operator switch
        {
            "*" => argument * operand,
            "/" => argument / operand,
            _ => argument
        };
    }
}
