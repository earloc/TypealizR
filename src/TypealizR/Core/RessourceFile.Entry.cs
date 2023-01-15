using System;
using System.Collections.Generic;
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

            var rawGroupKey = match.Groups["groupKey"].Value;

            if (string.IsNullOrEmpty(rawGroupKey))
            {
                Key = key;
                return;
            }

            Key = match.Groups["remainer"].Value.Trim(); 

            if (string.IsNullOrEmpty(Key))
            {
                Key = key;
            }

            Groups = Sanitize(rawGroupKey);
        }

        private IEnumerable<MemberName> Sanitize(string rawGroupKey)
        {
            var parts = rawGroupKey
                .Split('.')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray()
            ;

            return parts                .Select(x => new MemberName(x                    .ReplaceInvalidForMemberNameWith(' ')                    .Replace(" ", ""))                )                .ToArray();
        }

        public string Key { get; }
        public string RawKey { get; }
        public string Value { get; }

        internal IEnumerable<MemberName> Groups { get; } = Enumerable.Empty<MemberName>();

        public IXmlLineInfo Location { get; }
    }
}
