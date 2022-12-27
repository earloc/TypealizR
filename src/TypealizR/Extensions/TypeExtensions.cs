using System;
using System.Collections.Generic;
using System.Text;

namespace TypealizR.Extensions;
internal static class TypeExtensions
{
	internal static string GeneratedCodeAttribute(this Type that) => $@"[GeneratedCode(""{that.Name}"", ""{that.Assembly.GetName().Version}"")]";
}
