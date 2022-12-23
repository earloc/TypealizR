using Microsoft.CodeAnalysis;

namespace TypealizR.Tests.Snapshots;

internal class GeneratorTester : IVerifiable
{
    private readonly GeneratorDriver driver;
    public GeneratorTester(GeneratorDriver driver)
    {
        this.driver = driver;
    }

    public Task Verify() => Verifier.Verify(driver).UseDirectory(".snapshots");
}
