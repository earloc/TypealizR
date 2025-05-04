using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

public class EnumerateLocalizersSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../EnumerateLocalizersSourceGenerator.Tests";
    private const string RootNamespace = "Some.Root.Namespace";

    private static GeneratorTesterBuilder<EnumerateLocalizersSourceGenerator> Create() => 
        GeneratorTesterBuilder<EnumerateLocalizersSourceGenerator>
        .Create(BaseDirectory, RootNamespace, discoveryEnabled: true);

    [Fact]
    public async Task Generates_Extension_ForProperties() => await Create()
        .WithSourceFile("FooBar_Properties.cs")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Generates_Extension_ForConstructorArguments() => await Create()
        .WithSourceFile("FooBar_ConstructorArguments.cs")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Generates_Extension_ForFull() => await Create()
        .WithSourceFile("FooBar_Full.cs")
        .WithSource($$"""
            using Microsoft.Extensions.Localization;

            namespace FooBar.Extensions;

            public partial class FooBarExtensions
            {
                [EnumerateLocalizers]
                public partial IStringLocalizer[] GetAll(IServiceProvider sp);
            }
        """)
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
