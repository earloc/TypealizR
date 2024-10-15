using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = TypealizeR.Analyzer.Tests.CSharpCodeFixVerifier<
    TypealizeR.Analyzer.UseIndexerAnalyzer,
    TypealizeR.Analyzer.TypealizeRAnalyzerCodeFixProvider>;

namespace TypealizeR.Analyzer.Tests;

[TestClass]
public class UseIndexerAnalyzer_Test
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task Emits_NoDiagnostics_For_EmptySyntax()
    {
        var test = @"";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Emits_NoDiagnostics_For_MethodCallSyntax()
    {
        var test = $$"""
            public class Foo {
                public void Bar() {
                }
            }
            public class FooBar {
                public FooBar(Foo foo) {
                    foo.Bar();
                }
            }
        """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    string typeDeclarations = """

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
    

    string interfaceDeclaration = """
        namespace Microsoft.Extensions.Localization {
            public interface IStringLocalizer {
                LocalizedString this[string name] { get; }
                LocalizedString this[string name, params object[] arguments] { get; }
            }

            public interface IStringLocalizer<T> : IStringLocalizer {
            }
        }
    """;

    string generatedExtension = """
        namespace Microsoft.Extensions.Localization {
            public static class IStringLocalizerExtensions {
                public static LocalizedString Bar(this IStringLocalizer that) => that[nameof(Bar)];
                public static LocalizedString FooBar(this IStringLocalizer<Foo> that) => that[nameof(Bar)];
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

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task UseIndexSignature()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer.Bar|}();
                    }
                }
            }
        """);

        var expectedDiagnostics = VerifyCS.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer["Bar"]|};
                    }
                }
            }
        """);

        await VerifyCS.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task UseIndexSignature_Generic()
    {
        var test = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer<Foo> localizer) {
                        var x = {|#0:localizer.Bar|}();
                    }
                }
            }
        """);

        var expected = VerifyCS.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
