using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class StringLocalizerSourceGenerator_Tests
{
	private const string BaseDirectory = "../../../StringLocalizerSourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task Emits_Warning_TR0001()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithoutMsBuildProjectDirectory()
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
		Assert.Fail("this one here is a false positive");
	}

	[Fact]
	public async Task DoesNot_Emit_Warning_TR0001_When_Using_ProjectDir()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithoutMsBuildProjectDirectory(butWithProjectDir: BaseDirectory)
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
		Assert.Fail("this one here is a false positive");

	}

	[Fact]
	public async Task Emits_Warning_TR0002()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0002_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Emits_Warning_TR0003()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0003_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Emits_Warning_TR0004()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0004_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_Honors_Internal_MarkerType()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile($"NoWarnings_NoCode_Internal.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_Honors_Public_MarkerType()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile($"NoWarnings_NoCode_Public.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_MarkerType_Fallback()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile($"NoWarnings_NoCode_NamespaceMismatch.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}
}
