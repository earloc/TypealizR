﻿using System.Xml;

namespace TypealizR.Core;
public partial class RessourceFile
{
	public class Entry
    {
        public Entry(string key, string value, IXmlLineInfo location)
        {
			Key = key;
			Value = value;
			Location = location;
		}

		public string Key { get; }
		public string Value { get; }
		public IXmlLineInfo Location { get; }
	}
}
