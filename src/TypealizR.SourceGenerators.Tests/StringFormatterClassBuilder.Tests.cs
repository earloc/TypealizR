
using System.Globalization;
using FluentAssertions;
using TypealizR.SourceGenerators.Extensibility;

namespace TypealizR.SourceGenerators.Tests;
public class StringFormatterClassBuilder_Tests
{

	[Fact]
	public void Generates_DefaultImplementation_When_UserImplementation_Is_NOT_Provided()
	{
		var sut = new StringFormatterClassBuilder("Some.Name.Space");
		sut.UserModeImplementationIsProvided();

		var actual = sut.Build().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		var expected = $@"
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;

namespace Some.Name.Space {{
	internal static partial class {StringFormatterClassBuilder.TypeName} 
    {{
		public static partial string Format(this LocalizedString s, params object[] args);
	}}
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class {StringFormatterClassBuilder.TypeName} {{
		public static partial string Format(this LocalizedString s, params object[] args) => string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
	}}
}}
".Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		actual.Should().BeEquivalentTo(expected);

	}

	[Fact]
	public void DoesNot_Generates_DefaultImplementation_When_UserImplementation_IS_Provided()
	{
		var sut = new StringFormatterClassBuilder("Some.Name.Space");

		var actual = sut.Build().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		var expected = $@"
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;

namespace Some.Name.Space {{
	internal static partial class {StringFormatterClassBuilder.TypeName}
	{{
		public static partial string Format(this LocalizedString s, params object[] args);
	}}
}}
".Trim().Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		actual.Should().BeEquivalentTo(expected);

	}

}
