﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    TypealizeR.Analyzers.UseIndexerAnalyzer,
    TypealizeR.Analyzers.TypealizeRCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace TypealizeR.Analyzers.Tests;

[TestClass]
public class UseIndexerAnalyzer_Test
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task Emits_NoDiagnostics_For_EmptySyntax()
    {
        var test = @"";

        await Verify.VerifyAnalyzerAsync(test);
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
                public static LocalizedString Bar(this IStringLocalizer that) => that[nameof(Bar)];
                public static LocalizedString Bar_With_Foo(this IStringLocalizer that, string foo, string bar = "bar") => that[nameof(Bar), foo];
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

    [TestMethod]
    public async Task UseIndexSignature_OnParameter()
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

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");

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

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_OnLocal()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo() {
                        var localizer = GetLocalizer();
                        var x = {|#0:localizer.Bar|}();
                    }

                    private IStringLocalizer GetLocalizer() {
                        return null;
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo() {
                        var localizer = GetLocalizer();
                        var x = {|#0:localizer["Bar"]|};
                    }

                    private IStringLocalizer GetLocalizer() {
                        return null;
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_OnProperty()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public IStringLocalizer Localizer { get; set; }
                    public Foo() {
                        var x = {|#0:Localizer.Bar|}();
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public IStringLocalizer Localizer { get; set; }
                    public Foo() {
                        var x = {|#0:Localizer["Bar"]|};
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_OnMethod()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo() {
                        var x = {|#0:GetLocalizer().Bar|}();
                    }

                    private IStringLocalizer GetLocalizer() {
                        return null;
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo() {
                        var x = {|#0:GetLocalizer["Bar"]|};
                    }

                    private IStringLocalizer GetLocalizer() {
                        return null;
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_WithParameter_Literal_1()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer.Bar_With_Foo|}("foo");
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar_With_Foo");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer["Bar_With_Foo", "foo"]|};
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_WithParameter_Literal_2()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer.Bar_With_Foo|}("foo", "bar");
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar_With_Foo");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer["Bar_With_Foo", "foo", "bar"]|};
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_WithParameter_Local_1()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var s = "foo";
                        var x = {|#0:localizer.Bar_With_Foo|}(s);
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar_With_Foo");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var s = "foo";
                        var x = {|#0:localizer["Bar_With_Foo", s]|};
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

    [TestMethod]
    public async Task UseIndexSignature_WithParameter_Local_2()
    {
        var code = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var s = "foo";
                        var x = {|#0:localizer.Bar_With_Foo|}(s, "bar");
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar_With_Foo");

        var expectedCode = TestCode("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var s = "foo";
                        var x = {|#0:localizer["Bar_With_Foo", s, "bar"]|};
                    }
                }
            }
        """);

        await Verify.VerifyCodeFixAsync(code, expectedDiagnostics, expectedCode);
    }

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

        var expected = Verify.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");
        await Verify.VerifyAnalyzerAsync(test, expected);
    }
}
