using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class SourceGenerator_Tests
{
	[Fact]
	public async Task Generates_StringFormatter_WithDefaultImplementation_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create("../../../SourceGenerator.Tests", "Some.Root.Namespace")
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify();
	}

	[Fact]
	public async Task Generates_StringFormatter_BaseOnly_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create("../../../SourceGenerator.Tests", "Some.Root.Namespace")
			.WithSourceFile("StringFormatter.cs")
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify();
	}

	[Fact]
	public async Task Generates_ExtensionMethods_WithoutWarnings_For_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create("../../../SourceGenerator.Tests", "Some.Root.Namespace")
			.WithSourceFile("StringFormatter.cs")
			.WithResxFile("NoWarnings_NoCode.resx")
			.Build()
			.Verify();
	}
}
