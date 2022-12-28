using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Core;
public partial class RessourceFile
{
    public IEnumerable<Entry> Entries { get; }

    public RessourceFile(string simpleName, string fullPath, string content)
    {
        SimpleName = simpleName;
        FullPath = fullPath;
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
    public bool IsDefaultLocale { get; }
    
    public static IEnumerable<RessourceFile> From(ImmutableArray<AdditionalText> source) => From(source.Select(x => x.Path));

    public static IEnumerable<RessourceFile> From(IEnumerable<string> filePaths)
    {
		static string TryGetFileContent(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return string.Empty;
        }

        var byFolder = filePaths
            .GroupBy(x => Directory.GetParent(x).FullName)
            .Select(x => x.GroupBy(y => GetSimpleFileNameOf(y)))
        ;

        var files = byFolder
            .SelectMany(folder => folder
                .Select(resx => new { Name = resx.Key, MainFile = resx.Max()})
                .Select(_ => new RessourceFile(_.Name, _.MainFile, TryGetFileContent(_.MainFile))
            )
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
