using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Console;
internal static partial class TypealizR_StringFormatter
{
	internal static partial string Format(string s, object[] args) => new(string.Format(s, args).Reverse().ToArray());
}
