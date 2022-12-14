using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class TypealizedClassSourceGenerator_Tests
{
	private const string BaseDirectory = "../../../TypealizedClassSourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task NoCode_Resx_Groups_Are_Honored_In_Generated_Code()
	{
		await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile($"Groupings_NoCode.resx")
			.Build()
			.Verify()
		;
	}

	[Fact]
	public async Task NoCode_Resx_Groups_Handles_Duplicates()
	{
		await GeneratorTesterBuilder<TypealizedClassSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile($"Groupings_NoCode_WithDuplicates.resx")
			.Build()
			.Verify()
		;
	}
}
