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
	private const string BaseDirectory = "../../../SourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task Generates_Warning_TR0001()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithoutProjectDirectory()
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0002()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0002_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0003()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0003_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0004()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0004_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_StringFormatter_WithDefaultImplementation_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_StringFormatter_BaseOnly_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_ExtensionMethods_WithoutWarnings_For_NoCode_Resx()
	{
		await GeneratorTesterBuilder
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithResxFile("NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}
}
