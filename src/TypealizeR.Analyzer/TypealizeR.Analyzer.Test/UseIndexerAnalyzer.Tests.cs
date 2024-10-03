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

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task UseIndexer()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IFoo {
            void Bar();
        }

        class Foo
        {   
            public Foo(IFoo foo) {
                {|#0:foo.Bar|}();
            }
        }
    }";
            var expected = VerifyCS.Diagnostic(nameof(UseIndexerAnalyzer)).WithLocation(0).WithArguments("Bar");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
