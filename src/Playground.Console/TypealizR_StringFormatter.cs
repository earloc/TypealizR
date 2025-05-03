
namespace Playground.Console;
internal static partial class TypealizR_StringFormatter
{
    internal static partial string Format(string s, object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
    internal static partial string Extend(string argument, string extension) => extension switch
    {
        "toUpper" => argument?.ToString().ToUpper(System.Globalization.CultureInfo.CurrentCulture) ?? argument ?? "",
        _ => argument
    };

    internal static partial double Extend(double argument, string extension) => extension switch
    {
        "round0" => Math.Round(argument, 0),
        "round1" => Math.Round(argument, 1),
        "round2" => Math.Round(argument, 2),
        _ => argument
    };

    internal static partial int Extend(int argument, string extension)
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
