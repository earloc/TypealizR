
using System.Globalization;
using FluentAssertions;
using TypealizR.SourceGenerators.Extensibility;

namespace TypealizR.SourceGenerators.Tests;
public class StringFormatterClassBuilder_Tests
{

	[Fact]
	public void Generates_DefaultImplementation_When_UserImplementation_Is_Not_Provided()
	{
		var sut = new StringFormatterClassBuilder("Some.Name.Space");
		sut.UserModeImplementationIsProvided();

		var actual = sut.Build().Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		var expected = $@"
namespace global::Some.Name.Space {{
	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args);
	}}

	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class TypealizR_StringFormatter
	{{
		public static partial string Format(this LocalizedString s, params object[] args) => string.Format(global::System.Globalization.CultureInfo.CurrentCulture, s, args)
	}}
}}
".Trim().Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

		actual.Should().BeEquivalentTo(expected);

	}

}
