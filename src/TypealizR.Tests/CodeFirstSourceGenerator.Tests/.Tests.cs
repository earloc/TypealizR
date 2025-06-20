﻿using TypealizR.CodeFirst;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

public class CodeFirstSourceGenerator_Tests
{
    private const string BaseDirectory = "../../../CodeFirstSourceGenerator.Tests";

    [Fact]
    public async Task Does_Not_Care_About_NonMarked_Interfaces() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("ISomeInterface.cs")
            .Build()
            .Verify()
    ;

    [Fact]
    public async Task Uses_Default_Values_From_Member_Names() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("ITranslatables.cs")
            .Build()
            .Verify()
    ;

    [Fact]
    public async Task Honors_Members_With_Simple_Xml_Comment() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("IMembersWithSimpleXmlComment.cs")
            .Build()
            .Verify()
    ;

    [Fact]
    public async Task Honors_Methods_With_Parameters_In_Xml_Comment() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("IMethodsWithXmlCommentParameters.cs")
            .Build()
            .Verify()
    ;

    [Fact]
    public async Task Honors_Interfaces_Declared_As_Inner_Type() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("Innerface.cs")
            .Build()
            .Verify()
    ;

    [Fact]
    public async Task Honors_Interfaces_Accessibility() => await GeneratorTesterBuilder<CodeFirstSourceGenerator>
            .Create(BaseDirectory, null)
            .WithSourceFile("IPublicInterface.cs")
            .Build()
            .Verify()
    ;
}
