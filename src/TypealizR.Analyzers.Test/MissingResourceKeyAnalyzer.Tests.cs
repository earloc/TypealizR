using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using TypealizR.Diagnostics;
using Xunit;
using static TypealizR.Diagnostics.DiagnosticsId;
using static TypealizR.Analyzers.Tests.AnalyzerFixture<TypealizR.Analyzers.MissingResourceKeyAnalyzer>;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypealizR.Analyzers.Tests;

public record AnalyzerFixture<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    internal string ActualCode { get; set; } = "";

    internal static DiagnosticResult DiagnosticFor(DiagnosticsId id) => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(id.ToString());

    internal DiagnosticResult[] ExpectedDiagnostics { get; set; } = [];

    private Task RunAsync()
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>();

        var stringLocalizerType = typeof(IStringLocalizer);

        test.ReferenceAssemblies = test.ReferenceAssemblies.WithPackages([
            new(stringLocalizerType.Namespace!, stringLocalizerType.Assembly.GetName()?.Version?.ToString() ?? "*")
        ]);

        test.TestCode = ActualCode;

        var verifier = new CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>();
        test.ExpectedDiagnostics.AddRange(ExpectedDiagnostics);
        return test.RunAsync();
    }

    public TaskAwaiter GetAwaiter() => this.RunAsync().GetAwaiter();
}

public class MissingResourceKeyAnalyzer_Test(AnalyzerFixture<MissingResourceKeyAnalyzer> test) : IClassFixture<AnalyzerFixture<MissingResourceKeyAnalyzer>>
{
    [Fact]
    public async Task Emits_NoDiagnostics_For_EmptySyntax()
    {
        await (
            test with
            {
                ActualCode = ""
            }
        );
    }

    [Fact]
    public async Task Emits_NoDiagnostics_For_ElementAccessSyntax()
    {
        await (
            test with
            {
                ActualCode = $$"""
                    using {{typeof(Dictionary<string, string>).Namespace}};

                    public class FooBar {
                        private Dictionary<string, string> _foos = [];
                        public string Bar() {
                            return _foos[nameof(Bar)];
                        }
                    }
                """
            }
        );
    }

    [Fact]
    public async Task Emits_NoDiagnostics_For_MissingKey_Bar()
    {
        await (
            test with
            {
                ActualCode = $$"""
                    using {{typeof(IStringLocalizer).Namespace}};
                    namespace TypealizR.Analyzers.Tests;

                    public class Foo
                    {   
                        public Foo(IStringLocalizer<Foo> localizer) {
                            var x = {|#0:localizer["Bar"]|};
                        }
                    }
                """,
                ExpectedDiagnostics = [
                    DiagnosticFor(TR1010).WithLocation(0).WithArguments("Bar", "en")
                ]
            } 
        );
    }
}
