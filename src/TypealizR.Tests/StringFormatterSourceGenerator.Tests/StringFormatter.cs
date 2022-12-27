using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Root.Namespace;
internal static partial class TypealizR_StringFormatter
{
	internal static partial string Format(string s, object[] args) =>
		new string(string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args).Reverse());
}
