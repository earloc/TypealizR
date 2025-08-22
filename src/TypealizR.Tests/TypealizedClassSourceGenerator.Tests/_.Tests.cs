using Microsoft.CodeAnalysis;
using TypealizR.Core.Diagnostics;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

public class TypealizedClassSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../TypealizedClassSourceGenerator.Tests";
    private const string RootNamespace = "Some.Root.Namespace";

    [Fact]
    public async Task NoCode_Resx_Groups_Are_Honored_In_Generated_Code() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithResxFile($"Groupings_NoCode.resx")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task NoCode_Resx_Groups_Honors_Setting_UseParamNamesInMethodNames_BuildProperty() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace, useParamNamesInMethodNames: "false")
        .WithResxFile($"Groupings_NoCode.resx")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task NoCode_Resx_Groups_Honors_Setting_UseParamNamesInMethodNames_ItemMetadata() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithResxFile($"Groupings_NoCode.resx", useParamNamesInMethodNames: "false")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task NoCode_Resx_Groups_Handles_Duplicates() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithResxFile($"Groupings_NoCode_WithDuplicates.resx")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Emits_Warning_TR0005() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithResxFile("TR0005_NoCode.resx", useParamNamesInMethodNames: "false")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Emits_Error_TR0005() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithSeverityConfig(DiagnosticsId.TR0005, DiagnosticSeverity.Error)
        .WithResxFile("TR0005_NoCode.resx", useParamNamesInMethodNames: "false")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Resx_With_Blanks_SubstitutesSpaces() => await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
        .Create(BaseDirectory, RootNamespace)
        .WithResxFile("Simple   with Blanks.resx", useParamNamesInMethodNames: "false")
        .Build()
        .Verify()
    ;

    [Fact]
    public async Task Emits_Valid_XmlComments()
    {
        await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
            .Create(BaseDirectory, RootNamespace)
            .WithSeverityConfig(DiagnosticsId.TR0005, DiagnosticSeverity.Error)
            .WithResxFile("CS1570.resx", useParamNamesInMethodNames: "false")
            .Build()
            .Verify()
        ;
    }

    [Fact]
    public async Task Emits_EscapedDoubleQuotes_InKeys()
    {
        await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
            .Create(BaseDirectory, RootNamespace)
            .WithSeverityConfig(DiagnosticsId.TR0005, DiagnosticSeverity.Error)
            .WithResxFile("DoubleQuotesInKeys.resx", useParamNamesInMethodNames: "false")
            .Build()
            .Verify()
        ;
    }

}
