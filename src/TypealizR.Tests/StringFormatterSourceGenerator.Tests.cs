﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class StringFormatterSourceGenerator_Tests
{
	private const string BaseDirectory = "../../../SourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task Generates_Warning_TR0001()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithoutMsBuildProjectDirectory()
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task DoesNotGenerates_Warning_TR0001_When_Using_ProjectDir()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithoutMsBuildProjectDirectory(butWithProjectDir: BaseDirectory)
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0002()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0002_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0003()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0003_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_Warning_TR0004()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("TR0004_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_StringFormatter_WithDefaultImplementation_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_StringFormatter_BaseOnly_For_Empty_NoCode_Resx()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
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
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithResxFile("NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}
	[Fact]
	public async Task NoCode_Resx_Respects_Visibility_Of_Internal_MarkerType()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithSourceFile($"NoWarnings_NoCode_Internal.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_Respects_Visibility_Of_Public_MarkerType()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithSourceFile($"NoWarnings_NoCode_Public.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_Falls_Back_To_Inferred_MarkerType_When_Found_Ones_Namespace_Does_Not_Match()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.WithSourceFile($"NoWarnings_NoCode_NamespaceMismatch.cs")
			.WithResxFile($"NoWarnings_NoCode.resx")
			.Build()
			.Verify()
		;
	}
}
