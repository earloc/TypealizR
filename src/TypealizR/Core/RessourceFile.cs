using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace TypealizR.Core;
public partial class RessourceFile
{
    internal const string CustomToolNameSpaceItemMetadata = "build_metadata.embeddedresource.customtoolnamespace";
    internal const string UseParamNamesInMethodNamesBuildProperty = "build_property.typealizr_useparamnamesinmethodnames";
    internal const string UseParamNamesInMethodNamesItemMetadata = "build_metadata.embeddedresource.typealizr_useparamnamesinmethodnames";


    public IEnumerable<RessourceFileEntry> Entries { get; }

    public RessourceFile(string simpleName, string fullPath, string content, string? customToolNamespace, bool useParamNamesInMethodNames)
    {
        SimpleName = simpleName;
        FullPath = fullPath;
        CustomToolNamespace = !string.IsNullOrEmpty(customToolNamespace) ? customToolNamespace : null;
        UseParamNamesInMethodNames = useParamNamesInMethodNames;

        IsDefaultLocale = FullPath.EndsWith($"{simpleName}.resx", StringComparison.Ordinal);

        if (string.IsNullOrEmpty(content))
        {
            Entries = Enumerable.Empty<RessourceFileEntry>();
        }
        else
        {
            using var reader = new StringReader(content);

            var document = XDocument.Load(reader, LoadOptions.SetLineInfo);

            Entries = document
                .Root
                .Descendants()
                .Where(x => x.Name == "data")
                .Select(x => new RessourceFileEntry(
                    key: x.Attribute("name").Value,
                    value: x.Descendants("value").FirstOrDefault().Value,
                    location: x.Attribute("name")
                ));
        }
    }

    public string SimpleName { get; }
    public string FullPath { get; }
    public bool UseParamNamesInMethodNames { get; }
    public string? CustomToolNamespace { get; }
    public bool IsDefaultLocale { get; }

    public static IEnumerable<RessourceFile> From(ImmutableArray<AdditionalTextWithOptions> source, CancellationToken cancellationToken)
    {
        var byFolder = source
            .GroupBy(x => Path.GetDirectoryName(x.Text.Path))
            .Select(x => x.GroupBy(y => GetSimpleFileNameOf(y.Text.Path)))
        ;

        var files = byFolder
            .SelectMany(folder => folder
                .Select(resx => new { Name = resx.Key, MainFile = resx.FirstOrDefault(x => x.Text.Path.EndsWith($"{resx.Key}.resx", StringComparison.Ordinal)) })
                .Where(_ => _.MainFile is not null)
                .Select(_ =>
                {
                    _.MainFile.Options.TryGetValue(CustomToolNameSpaceItemMetadata, out var customToolNamespace);

                    var useParamNamesInMethodNames = true;

                    if (_.MainFile.Options.TryGetValue(UseParamNamesInMethodNamesBuildProperty, out var useParamNamesInMethodNamesBuildPropertyString)
                        && !string.IsNullOrEmpty(useParamNamesInMethodNamesBuildPropertyString)
                        && bool.TryParse(useParamNamesInMethodNamesBuildPropertyString, out var useParamNamesInMethodNamesBuildProperty))
                    {
                        useParamNamesInMethodNames = useParamNamesInMethodNamesBuildProperty;
                    }

                    if (_.MainFile.Options.TryGetValue(UseParamNamesInMethodNamesItemMetadata, out var useParamNamesInMethodNamesItemMetadataString)
                        && !string.IsNullOrEmpty(useParamNamesInMethodNamesItemMetadataString)
                        && bool.TryParse(useParamNamesInMethodNamesItemMetadataString, out var useParamNamesInMethodNamesItemMetadata))
                    {
                        useParamNamesInMethodNames = useParamNamesInMethodNamesItemMetadata;
                    }

                    var content = cancellationToken.IsCancellationRequested ? string.Empty : _.MainFile.Text.GetText()?.ToString() ?? string.Empty;

                    return new RessourceFile(
                        simpleName: _.Name,
                        fullPath: _.MainFile.Text.Path,
                        content: content,
                        customToolNamespace: customToolNamespace,
                        useParamNamesInMethodNames: useParamNamesInMethodNames
                    );
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
