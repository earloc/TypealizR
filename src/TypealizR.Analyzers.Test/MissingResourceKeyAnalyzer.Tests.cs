using System.Threading.Tasks;
using Xunit;
using static TypealizR.Diagnostics.DiagnosticsId;

using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    TypealizR.Analyzers.MissingResourceKeyAnalyzer,
    TypealizR.Analyzers.TypealizRCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace TypealizR.Analyzers.Tests;

public class MissingResourceKeyAnalyzer_Test
{

    private readonly StringLocalizerTestCodeBuilder test = new();

    [Fact]
    public async Task Emits_NoDiagnostics_For_EmptySyntax()
    {
        var test = @"";

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task Emits_NoDiagnostics_For_ElementAccessSyntax()
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

    [Fact]
    public async Task Reports_MissingKey_Bar()
    {
        var code = test.Code("""
            namespace ConsoleApplication1 {
                public class Foo
                {   
                    public Foo(IStringLocalizer localizer) {
                        var x = {|#0:localizer["Bar"]|};
                    }
                }
            }
        """);

        var expectedDiagnostics = Verify.Diagnostic(TR1010.ToString()).WithLocation(0).WithArguments("Bar", "en");

        await Verify.VerifyAnalyzerAsync(code, expectedDiagnostics);
    }
}
