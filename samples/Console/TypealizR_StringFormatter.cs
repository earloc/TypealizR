using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace CLI;
internal static partial class TypealizR_StringFormatter
{
	internal static partial string Format(string s, object[] args) => new(string.Format(s, args).Reverse().ToArray());
    internal static partial object ExtendArg(object that, string extension) => extension switch
    {
        "MyCustomExtension" => that?.ToString()?.ToLower() ?? that,
        _ => that
    };
}
