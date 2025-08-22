using System.Security;
using System.Xml.Linq;

namespace TypealizR.Core;
internal class CommentModel
{
    private readonly ResourceKey key;
    private readonly XText defaultValue;

    public CommentModel(ResourceKey key, string defaultValue)
    {
        this.key = key;
        this.defaultValue = new XText(defaultValue);
    }

    public string ToCSharp() => $"""

            /// <summary>
            /// Looks up a localized string similar to '{key.Value}'
            /// </summary>
            /// <returns>
            /// A localized version of the current default value of '{defaultValue}'
            /// </returns>
    """;
}
