using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypealizR.Core;
public partial class RessourceFile
{
    internal const string CustomToolNameSpaceProperty = "build_metadata.embeddedresource.customtoolnamespace";

    public IEnumerable<Entry> Entries { get; }

    public RessourceFile(string simpleName, string fullPath, string content, string? customToolNamespace)
    {
        SimpleName = simpleName;
        FullPath = fullPath;        CustomToolNamespace = !string.IsNullOrEmpty(customToolNamespace) ? customToolNamespace : null;

        IsDefaultLocale = FullPath.EndsWith($"{simpleName}.resx");

        if (string.IsNullOrEmpty(content))
        {
            Entries = Enumerable.Empty<Entry>();
        }
        else
        {
            using var reader = new StringReader(content);

            var document = XDocument.Load(reader, LoadOptions.SetLineInfo);

            Entries = document
                .Root
                .Descendants()
                .Where(x => x.Name == "data")
                .Select(x => new Entry(
                    key: x.Attribute("name").Value,
                    value: x.Descendants("value").FirstOrDefault().Value,
					location: x.Attribute("name")
				));
        }
    }

    public string SimpleName { get; }
    public string FullPath { get; }
    public string? CustomToolNamespace { get; }
    public bool IsDefaultLocale { get; }
    
    public static IEnumerable<RessourceFile> From(ImmutableArray<AdditionalTextWithOptions> source, CancellationToken cancellationToken)
    {
        var byFolder = source
            .GroupBy(x => Directory.GetParent(x.Text.Path).FullName)
            .Select(x => x.GroupBy(y => GetSimpleFileNameOf(y.Text.Path)))
        ;

        var files = byFolder
            .SelectMany(folder => folder
                .Select(resx => new { Name = resx.Key, MainFile = resx.Max() })
                .Select(_ => {
                    _.MainFile.Options.TryGetValue(CustomToolNameSpaceProperty, out var customToolNamesapce);
                    return new RessourceFile(_.Name, _.MainFile.Text.Path, _.MainFile.Text.GetText(cancellationToken)?.ToString() ?? string.Empty, customToolNamesapce);
                })
        );

        return files.Where(x => x.IsDefaultLocale);
    }

    internal static string GetSimpleFileNameOf(string input)
    {
        var fileNameWithoutLastExtension = Path.GetFileNameWithoutExtension(input);
        var fileNameWithoutAdditionalExtension = Path.GetFileNameWithoutExtension(fileNameWithoutLastExtension);

        while (fileNameWithoutLastExtension != fileNameWithoutAdditionalExtension)
        {
            fileNameWithoutLastExtension = fileNameWithoutAdditionalExtension;
            fileNameWithoutAdditionalExtension = Path.GetFileNameWithoutExtension(fileNameWithoutAdditionalExtension);
        }

        return fileNameWithoutAdditionalExtension;
    }
}
