using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace TypealizR.Core;
public partial class RessourceFile
{
	public class Entry
    {
		private readonly Regex epression = new(@"^ ?\[(?<groupKey>.*)\]:(?<remainer>[^$].*)", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        public Entry(string key, string value, IXmlLineInfo location)
        {
			RawKey = key;
			Value = value;
			Location = location;

			var match = epression.Match(key);

			var rawGroupKey = match.Groups["groupKey"]?.Value ?? "";

			if (string.IsNullOrEmpty(rawGroupKey))
			{
				Key = key;
				return;
			}

			Key = (match.Groups["remainer"]?.Value ?? "").Trim(); 

			if (string.IsNullOrEmpty(Key))
			{
				Key = key;
			}

			GroupKey = Sanitize(rawGroupKey);
		}

		private string? Sanitize(string rawGroupKey)
		{
			var parts = rawGroupKey
				.Split('.')
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrEmpty(x))
				.ToArray()
			;

			return string.Join(".", parts);
		}

		public string Key { get; }
		public string RawKey { get; }
		public string Value { get; }

		public string? GroupKey { get; }

		public IXmlLineInfo Location { get; }
	}
}
