using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace TypealizR.SourceGenerators.Playground.Console;
internal static partial class TypealizR_StringFormatter
{
	public static partial LocalizedString Format(this LocalizedString that, params object[] args)
	{
		var value = string.Format(that.Value, args);
		return new(that.Name, new string(value.Reverse().ToArray()), that.ResourceNotFound, that.SearchedLocation);
	}
}
