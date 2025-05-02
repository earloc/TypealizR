using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

public class DiscoverUsageSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../DiscoverUsageSourceGenerator.Tests";
    private const string RootNamespace = "Some.Root.Namespace";

    [Fact]
    public async Task Generates_Empty_Extension() => await GeneratorTesterBuilder<DiscoverUsageSourceGenerator>
        .Create(BaseDirectory, RootNamespace, discoveryEnabled: true)
        .WithSourceFile("FooBar.cs")
        .Build()
        .Verify()
    ;

    // [Fact]
    // public async Task Generates_StringFormatter_Stub_Only_When_UserImplementation_Is_Provided() => await GeneratorTesterBuilder<DiscoverUsageSourceGenerator>
    //         .Create(BaseDirectory, RootNamespace)
    //         .WithSourceFile("StringFormatter.cs")
    //         .Build()
    //         .Verify()
    //     ;
}
