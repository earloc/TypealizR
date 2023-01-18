using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class CodeFirstSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../CodeFirstSourceGenerator.Tests";

    [Fact]
    public async Task Throws_When_RootNamespace_Is_Missing()
    {
        await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("Interfaces.cs")
            .Build()
            .Verify()
        ;
    }
}
