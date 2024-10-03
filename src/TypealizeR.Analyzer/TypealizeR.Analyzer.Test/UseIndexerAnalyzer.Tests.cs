using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = TypealizeR.Analyzer.Test.CSharpCodeFixVerifier<
    TypealizeR.Analyzer.UseIndexerAnalyzer,
    TypealizeR.Analyzer.TypealizeRAnalyzerCodeFixProvider>;

namespace TypealizeR.Analyzer.Test
{
    [TestClass]
    public class UseIndexerAnalyzer_Test
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }


        string interfaceDeclaration = """
            namespace Microsoft.Extensions.Localization {
                    public interface IStringLocalizer {
                    }
                    public static class IStringLocalizerExtensions {
                        public static void Bar(this IStringLocalizer that) {
                        }
                    }
                }
        """;


        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task UseIndexer()
        {
            var test = $$"""
                using Microsoft.Extensions.Localization;
            
                {{interfaceDeclaration}}

                namespace ConsoleApplication1
                {
                    class Foo
                    {   
                        public Foo(IStringLocalizer localizer) {
                            {|#0:localizer.Bar|}();
                        }
                    }
                }
            """;

            var expected = VerifyCS.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
