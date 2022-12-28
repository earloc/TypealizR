using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using TypealizR.Builder;
using TypealizR.Diagnostics;
using TypealizR.Tests.Snapshots;

namespace TypealizR.Tests;

[UsesVerify]
public class StringTypealizRSourceGenerator_Tests
{
	private const string BaseDirectory = "../../../StringTypealizRSourceGenerator.Tests";
	private const string RootNamespace = "Some.Root.Namespace";

	[Fact]
	public async Task NoCode_Resx_Groups_Are_Honored_In_Generated_Code()
	{
		await GeneratorTesterBuilder<StringTypealizRSourceGenerator>
			.Create(BaseDirectory, RootNamespace)
			.WithResxFile($"Groupings_NoCode.resx")
			.Build()
			.Verify()
		;
	}
}
