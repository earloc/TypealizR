using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TypealizR.Builder;
internal class CommentModel
{
	private readonly string rawResourceName;
	private readonly string defaultValue;

	public CommentModel(string rawResourceName, string defaultValue)
	{
		this.rawResourceName = rawResourceName;
		this.defaultValue = defaultValue;
	}

	public string ToCSharp() => $"""
			/// <summary>
			/// Looks up a localized string similar to '{rawResourceName}'
			/// </summary>
			/// <returns>
			/// A localized version of the current default value of '{defaultValue}'
			/// </returns>
	""";
}
