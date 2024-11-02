using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static TypealizR.Diagnostics.DiagnosticsId;

using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    TypealizR.Analyzers.UseIndexerAnalyzer,
    TypealizR.Analyzers.TypealizRCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace TypealizR.Analyzers.Tests;

[TestClass]
public class MissingResourceKeyAnalyzer_Test
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task Emits_NoDiagnostics_For_EmptySyntax()
    {
        var test = @"";

        await Verify.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Emits_NoDiagnostics_For_IndexerSyntax()
    {
        var test = $$"""
            using System.Collections.Generic;

            public class FooBar {
                private Dictionary<string, string> _foos = [];
                public string Bar() {
                    return _foos[nameof(Bar)];
                }
            }
        """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    readonly string typeDeclarations = """

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

    readonly string interfaceDeclaration = """
        namespace Microsoft.Extensions.Localization {
            public interface IStringLocalizer {
                LocalizedString this[string name] { get; }
                LocalizedString this[string name, params object[] arguments] { get; }
            }

            public interface IStringLocalizer<T> : IStringLocalizer {
            }
        }
    """;

    readonly string generatedExtension = """
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

    private string TestCode(string testCode)
    {
        var test = $$"""
            using Microsoft.Extensions.Localization;

            {{typeDeclarations}}
            {{interfaceDeclaration}}
            {{generatedExtension}}

            {{testCode}}
        """;

        return test;
    }

    //[TestMethod]
    //public async Task UseIndexSignature_OnParameter()
    //{
    //    var code = TestCode("""
    //        namespace ConsoleApplication1 {
    //            public class Foo
    //            {   
    //                public Foo(IStringLocalizer localizer) {
    //                    var x = {|#0:localizer.Bar|}();
    //                }
    //            }
    //        }
    //    """);

    //    var expectedDiagnostics = Verify.Diagnostic(TR1000.ToString()).WithLocation(0).WithArguments("Bar");

    //    var expectedCode = TestCode("""
    //        namespace ConsoleApplication1 {
    //            public class Foo
    //            {   
    //                public Foo(IStringLocalizer localizer) {
    //                    var x = {|#0:localizer["Bar"]|};
    //                }
    //            }
    //        }
    //    """);

    //    await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    //}
}
