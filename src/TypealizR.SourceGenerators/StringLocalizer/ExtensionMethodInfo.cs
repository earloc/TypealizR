using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TypealizR.SourceGenerators.StringLocalizer;
internal class ExtensionMethodInfo
{
    public static string ThisParameterFor(string T) => $"this IStringLocalizer<{T}> that";

    private readonly string T;
    private readonly string rawRessourceName;
    private readonly string defaultValue;
    public readonly string Name;
    public readonly IEnumerable<ExtensionMethodParameterInfo> Parameters;
    public readonly string Signature;
    public readonly string Body;

    public ExtensionMethodInfo(string tOfStringLocalizer, string rawRessourceName, string defaultValue, string compilableMethodName, IEnumerable<ExtensionMethodParameterInfo>? parameters = null)
    {
        T = tOfStringLocalizer;
        this.rawRessourceName = rawRessourceName;
        this.defaultValue = defaultValue;
        Name = compilableMethodName;
        Parameters = parameters ?? Enumerable.Empty<ExtensionMethodParameterInfo>();

        Signature = $"({ThisParameterFor(T)})";
        Body = $@"that[""{rawRessourceName}""]";

        if (Parameters.Any())
        {
            var additionalParameterDeclarations = string.Join(", ", Parameters.Select(x => x.Declaration));
            Signature = $"({ThisParameterFor(T)}, {additionalParameterDeclarations})";

            var parameterCollection = string.Join(", ", Parameters.Select(x => x.Name));
            Body = $@"that[""{rawRessourceName}"", {parameterCollection}]";
        }
    }

    public string Declaration => $@"  
  /// <summary>
  /// Looks up a localized string similar to '{rawRessourceName}'
  /// </summary>
  /// <returns>
  /// A localized version of the current default value of '{defaultValue.Replace("\r\n", " ").Replace("\n", " ")}'
  /// </returns>
  public static string {Name}{Signature} => {Body};
";
}
