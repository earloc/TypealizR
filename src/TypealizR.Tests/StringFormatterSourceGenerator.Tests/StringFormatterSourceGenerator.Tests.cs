using System;
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
	private const string BaseDirectory = "../../../StringFormatterSourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task Generates_StringFormatter_With_Default_Implementation()
	{
		await GeneratorTesterBuilder<StringFormatterSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task Generates_StringFormatter_Stub_Only_When_UserImplementation_Is_Provided()
	{
		await GeneratorTesterBuilder<StringFormatterSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithSourceFile("StringFormatter.cs")
			.Build()
			.Verify()
		;
	}
}
