﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators;
internal class RessourceFile
{

    public RessourceFile(string simpleName, string fullPath)
    {
        SimpleName = simpleName;
        FullPath = fullPath;
        IsDefaultLocale = FullPath.EndsWith($"{simpleName}.resx");
    }

    public string SimpleName { get; }

    public string FullPath { get; }
    public bool IsDefaultLocale { get; }

    public static IEnumerable<RessourceFile> From(ImmutableArray<AdditionalText> source) => From(source.Select(x => x.Path));

    public static IEnumerable<RessourceFile> From(IEnumerable<string> filePaths)
    {
        var byFolder = filePaths
            .GroupBy(x => Directory.GetParent(x).FullName)
            .Select(x => x.GroupBy(y => GetSimpleFileNameOf(y)))
        ;

        var files = byFolder
            .SelectMany(folder => folder
                .Select(resx => new RessourceFile(resx.Key, resx.Max())
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