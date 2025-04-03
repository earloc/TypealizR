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
    internal static partial object ExtendArg(object that, string extension) => extension switch
    {
        "toLower" => that?.ToString().ToLower() ?? that,
        _ => that
    };
}
