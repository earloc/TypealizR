using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using TypealizR.Diagnostics;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class StringLocalizerSourceGenerator_Tests
{
	private const string BaseDirectory = "../../../StringLocalizerSourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";


	[Fact]
	public async Task Throws_When_RootNamespace_Is_Missing()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, null)
			.WithResxFile("Empty_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Throws_For_Invalid_SeverityConfig()
	{
		await this.Invoking(async (x) =>
			await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
				.Create(BaseDirectory, null)
				.WithSeverityConfig(DiagnosticsId.TR0001, "invalid")
				.WithResxFile("Empty_NoCode.resx")
				.Build()
				.Verify()
		)
		.Should()
		.ThrowAsync<InvalidOperationException>()
		.WithMessage("'dotnet_diagnostic_tr0001_severity' has invalid value 'invalid'")
		;
	}


	[Fact]
	public async Task Emits_Error_TR0001()
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
	public async Task DoesNot_Emit_Warning_TR0001_When_Using_ProjectDir()
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
	public async Task Emits_Error_TR0002()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSeverityConfig(DiagnosticsId.TR0002, DiagnosticSeverity.Error)
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
	public async Task Emits_Error_TR0003()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSeverityConfig(DiagnosticsId.TR0003, DiagnosticSeverity.Error)
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
	public async Task Emits_Error_TR0004()
	{
		await GeneratorTesterBuilder<StringLocalizerSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSeverityConfig(DiagnosticsId.TR0004, DiagnosticSeverity.Error)
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
