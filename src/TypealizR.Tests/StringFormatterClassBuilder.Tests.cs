
using System.Globalization;
using FluentAssertions;
using TypealizR.Extensibility;

namespace TypealizR.Tests;
public class StringFormatterClassBuilder_Tests
{

	[Fact]
	public void Generates_DefaultImplementation_When_UserImplementation_Is_NOT_Provided()
	{
		var sut = new StringFormatterClassBuilder("Some.Name.Space");
		sut.UserModeImplementationIsProvided();

		var actual = sut.Build().TrimWrap();

		var expected = $@"
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;

namespace Some.Name.Space {{
	internal static partial class {StringFormatterClassBuilder.TypeName}
	{{
		internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
			new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

		internal static partial string Format(string s, object[] args);
	}}

	{_.GeneratedCodeAttribute}
	[DebuggerStepThrough]
	internal static partial class {StringFormatterClassBuilder.TypeName} {{
		internal static partial string Format(string s, object[] args) => 
			string.Format(System.Globalization.CultureInfo.CurrentCulture, s, args);
	}}
}}
".Trim().TrimWrap();

		actual.Should().BeEquivalentTo(expected);

	}

	[Fact]
	public void DoesNot_Generates_DefaultImplementation_When_UserImplementation_IS_Provided()
	{
		var sut = new StringFormatterClassBuilder("Some.Name.Space");

		var actual = sut.Build().TrimWrap();

		var expected = $@"
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Localization;

namespace Some.Name.Space {{
	internal static partial class {StringFormatterClassBuilder.TypeName}
	{{
		internal static LocalizedString Format(this LocalizedString that, params object[] args) => 
			new LocalizedString(that.Name, Format(that.Value, args), that.ResourceNotFound, searchedLocation: that.SearchedLocation);

		internal static partial string Format(string s, object[] args);
	}}
}}
".Trim().TrimWrap();

		actual.Should().BeEquivalentTo(expected);
	}
}
