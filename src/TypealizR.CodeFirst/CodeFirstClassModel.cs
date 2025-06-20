﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TypealizR.Core;
using TypealizR.Extensions;

namespace TypealizR.CodeFirst;

internal sealed class CodeFirstClassModel(string fileName, TypeModel implementingInterface, TypeModel type, string[] containingTypes, IEnumerable<CodeFirstMethodModel> methods, IEnumerable<CodeFirstPropertyModel> properties)
{
    private static CultureInfo codeCulture = new("en-US", false);
    private readonly TypeModel implementingInterface = implementingInterface;
    private readonly TypeModel type = type;
    private readonly string[] containingTypes = containingTypes;
    private readonly IEnumerable<CodeFirstMethodModel> methods = methods;
    private readonly IEnumerable<CodeFirstPropertyModel> properties = properties;
    private readonly HashSet<string> usings =
    [
        "System",
        "System.CodeDom.Compiler",
        "System.Diagnostics",
        "System.Diagnostics.CodeAnalysis",
        "Microsoft.Extensions.Localization"
    ];

    public string FileName { get; } = fileName;

    internal string ToCSharp(Type generatorType)
    {
        var builder = new StringBuilder();
        int indentation = 0;
        var spaces = new string(' ', indentation);

        builder.AppendLine($$"""
        // <auto-generated/>
        {{usings.Select(x => $"using {x};").ToMultiline(prependLinesWith: spaces, appendNewLineAfterEach: false)}}
        namespace {{type.Namespace}} {
        """);

        foreach (var containingType in containingTypes)
        {
            indentation += 4;
            spaces = new string(' ', indentation);
            builder.AppendLine($$"""{{spaces}}partial class {{containingType}} {""");
        }
        var accessibility = implementingInterface.Accessibility.ToVisibilty().ToString().ToLower(codeCulture);

        builder.AppendLine(GenerateImplementationType(generatorType, spaces, accessibility));

        foreach (var containingType in containingTypes)
        {
            builder.AppendLine($$"""{{spaces}}}""");
            indentation -= 4;
            spaces = new string(' ', indentation);
        }
        builder.AppendLine($$"""}""");

        return builder.ToString();
    }

    private string GenerateImplementationType(Type generatorType, string spaces, string accessibility) => $$"""
    {{spaces}}    {{generatorType.GeneratedCodeAttribute()}}
    {{spaces}}    {{accessibility}} partial class {{type.Name}}: {{implementingInterface.Name}} {
    {{spaces}}        private readonly IStringLocalizer<{{implementingInterface.Name}}> localizer;
    {{spaces}}        public {{type.Name}} (IStringLocalizer<{{implementingInterface.Name}}> localizer) {
    {{spaces}}          this.localizer = localizer;
    {{spaces}}        }
    {{spaces}}        #region methods

    {{methods.Select(x => x.ToCSharp(spaces)).ToMultiline($"{spaces}            ", appendNewLineAfterEach: false)}}

    {{spaces}}        #endregion

    {{spaces}}        #region properties

    {{properties.Select(x => x.ToCSharp(spaces)).ToMultiline($"{spaces}    ", appendNewLineAfterEach: false)}}

    {{spaces}}        #endregion

    {{spaces}}    }
    """;
}
