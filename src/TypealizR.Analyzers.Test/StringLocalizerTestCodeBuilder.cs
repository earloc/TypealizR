using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypealizR.Analyzers.Tests;
internal class StringLocalizerTestCodeBuilder
{
    private string generatedExtension = "";

    public StringLocalizerTestCodeBuilder WithExtensionMethods()
    {
        generatedExtension = """
            namespace Microsoft.Extensions.Localization {
                public static class IStringLocalizerExtensions {
                    public static LocalizedString Bar(this IStringLocalizer that) => that[_Bar];
                    private const string _Bar = "Bar";

                    public static LocalizedString Bar_With_Foo(this IStringLocalizer that, string foo, string bar = "bar") => that[_Bar_With_Foo, foo];
                    private const string _Bar_With_Foo = "Bar_With_Foo";

                    public static LocalizedString FooBar(this IStringLocalizer<Foo> that) => that[_FooBar];
                    private const string _FooBar = "FooBar";
                }
            }
        """;
        return this;
    }


    private const string typeDeclarations = """

        public class Foo {
        }

        namespace Microsoft.Extensions.Localization {
            public class LocalizedString {
                public string Name { get; } = "";
                public string Value { get; } = "";
                public bool ResourceNotFound { get; } = false;
                public string SearchedLocation { get; } = null;
            }
        }
    """;

    private const string interfaceDeclaration = """
        namespace Microsoft.Extensions.Localization {
            public interface IStringLocalizer {
                LocalizedString this[string name] { get; }
                LocalizedString this[string name, params object[] arguments] { get; }
            }

            public interface IStringLocalizer<T> : IStringLocalizer {
            }
        }
    """;



    internal string Code(string code)
    {
        var testCode = $$"""
            using Microsoft.Extensions.Localization;

            {{typeDeclarations}}
            {{interfaceDeclaration}}
            {{generatedExtension}}

            {{code}}
        """;

        return testCode;
    }
}
