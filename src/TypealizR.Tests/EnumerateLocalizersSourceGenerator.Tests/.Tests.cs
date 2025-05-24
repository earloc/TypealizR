using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

public class EnumerateLocalizersSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../EnumerateLocalizersSourceGenerator.Tests";
    private const string RootNamespace = "Some.Root.Namespace";

    private static GeneratorTesterBuilder<EnumerateLocalizersSourceGenerator> Create() => 
        GeneratorTesterBuilder<EnumerateLocalizersSourceGenerator>
        .Create(BaseDirectory, RootNamespace, discoveryEnabled: true)
    ;

    private static readonly string staticExtensionStaticMethod = $$"""
        namespace FooBar.Extensions;

        internal partial class FooBarExtensions
        {
            [EnumerateLocalizers]
            internal static partial IEnumerable<IStringLocalizer> GetAll(IServiceProvider sp);
        }
    """;

    [Fact]
    public async Task Generates_Extension_ForProperties() => await Create()
        .WithSource(staticExtensionStaticMethod)
        .WithSourceFile("FooBar_Properties.cs")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Generates_Extension_ForConstructorArguments() => await Create()
        .WithSource(staticExtensionStaticMethod)
        .WithSourceFile("FooBar_ConstructorArguments.cs")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Generates_Extension_ForFull() => await Create()
        .WithSource(staticExtensionStaticMethod)
        .WithSourceFile("FooBar_Full.cs")
        .Build()
        .Verify()
    ;

     [Fact]
    public async Task Generates_Extension_ForRazor() => await Create()
        .WithSource(staticExtensionStaticMethod)
        .WithRazor("FooPage.razor", $$"""
            @page "FooPage"
            @using Foo.Bar
            @inject IStringLocalizer<Foo> fooCalizer

            @code
            {
                private IStringLocalizer<Bar>? barCalizer = null;
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
