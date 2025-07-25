using System.Security;
using System.Xml.Linq;

namespace TypealizR.Core;
internal class CommentModel
{
    private readonly string rawResourceName;
    private readonly XText defaultValue;

    public CommentModel(string rawResourceName, string defaultValue)
    {
        this.rawResourceName = rawResourceName;
        this.defaultValue = new XText(defaultValue);
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
