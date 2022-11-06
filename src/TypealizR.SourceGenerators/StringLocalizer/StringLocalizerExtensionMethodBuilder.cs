using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TypealizR.SourceGenerators.Extensions;

namespace TypealizR.Extensions.System;

public partial class StringLocalizerExtensionMethodBuilder
{
    private string ressourceName;
    private readonly string defaultValue;

    public StringLocalizerExtensionMethodBuilder(string ressourceName, string defaultValue)
    {
        this.ressourceName = ressourceName;
        this.defaultValue = defaultValue;
    }

    private string SanitizeMethodName(string rawMethodName)
    {
        return new string(
            rawMethodName
                .Replace(" ", "_")
                .Trim('_')
                .Select((x, i) => x.IsValidInIdentifier(i == 0) ? x : ' ')
                .ToArray()
        )
        .Replace(" ", "")
        .Trim('_');
    }

}