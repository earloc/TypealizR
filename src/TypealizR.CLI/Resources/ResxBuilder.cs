using System.Xml.Linq;

namespace TypealizR.CLI.Resources;
internal class ResxBuilder
{
    private const string resHeader = "resheader";
    private const string value = "value";
    private readonly Dictionary<string, string> entries = new();
    public ResxBuilder Add(string key, string value)
    {
        entries.Add(key, value);
        return this;
    }

    public string Build()
    {
        var document = new XDocument(
            new XElement("root",
                new XElement(resHeader, new XAttribute("name", "resmimetype"),
                    new XElement(value, "text/microsoft-resx")
                ),
                new XElement(resHeader, new XAttribute("name", "version"),
                    new XElement("value", "2.0")
                ),
                new XElement(resHeader, new XAttribute("name", "reader"),
                    new XElement("value", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                ),
                new XElement(resHeader, new XAttribute("name", "writer"),
                    new XElement("value", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                ),
                entries.Select(x =>
                    new XElement("data",
                            new XAttribute("name", x.Key),
                        new XElement("value", x.Value)
                    )
                ).ToArray()
            )
        );

        return document.ToString();
    }
}
