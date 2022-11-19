using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

namespace TypealizR.SourceGenerators.Tests;

public class DiagnosticsFactoryTests
{

	private DiagnosticsFactory CreateSut(DiagnosticsId id) => new("someFile.resx", "someKey", 42);


	[Fact]
	public void DoestNot_Emit_Warning_When_Disabled_TR0002 ()
	{
		var sut = CreateSut(DiagnosticsFactory.TR0001);

		var diagnostic = sut.AmbigiousRessourceKey_0002("fallback");
		diagnostic.DefaultSeverity.Should().Be(Microsoft.CodeAnalysis.DiagnosticSeverity.Info);
	}
}
