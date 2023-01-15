using FluentAssertions;
using TypealizR.Core;

namespace TypealizR.Tests;

public class ExtensionMethodBuilder_Tests
{
	private static readonly TypeModel targetType = new("Name.Space", "TypeName");

	[Theory]
	[InlineData("Name", "Name")]
	[InlineData("Hello World!", "Hello_World")]
	[InlineData("Hello World.", "Hello_World")]
	[InlineData("Hello World?", "Hello_World")]
	[InlineData("Hello {0}", "Hello__0")]
	[InlineData("{0} Hello {0}", "Hello__0")]
	[InlineData("Hello {name}", "Hello__name")]
	[InlineData("Hello {name}, today is {now}", "Hello__name__today_is__now")]
	[InlineData("{name} Hello {name}, today is {now}", "name__Hello__name__today_is__now")]
	[InlineData("{now} Hello {name}, today is {now}", "now__Hello__name__today_is__now")]
	[InlineData("{timestamp} Hello {name}, today is {now}", "timestamp__Hello__name__today_is__now")]
	[InlineData("Hello {name:s}, today is {now:d}", "Hello__name__today_is__now")]
	public void Ensures_Compilable_ExtensionMethodName(string input, string expected)
	{
		var sut = new ExtensionMethodBuilder(targetType, input, input, new("Ressource1.resx", input, 42));
		var method = sut.Build();

		var actual = method.Name.ToString();
		actual.Should().Be(expected);
	}

    [Theory]
    [InlineData("Greet",                    "Greet")]
    [InlineData("Greet {0}",                "Greet")]
    [InlineData("{0} Greet {0}",            "Greet")]
    [InlineData("Greet {name}",             "Greet")]
    [InlineData("Greet {name:s}",           "Greet")]
    [InlineData("Greet{name}",              "Greet")]
    [InlineData("Greet{name:s}",            "Greet")]
    [InlineData("Greet{name} ",             "Greet")]
    [InlineData("Greet{name:s} ",           "Greet")]

    [InlineData("Greet {name} {now}",       "Greet")]
    [InlineData("Greet {name:s} {now:d}",   "Greet")]

    [InlineData("Greet {name}, {now}",      "Greet")]
    [InlineData("Greet {name:s}, {now:d}",  "Greet")]

    [InlineData("Greet {name}{now}",        "Greet")]
    [InlineData("Greet {name:s}{now:d}",    "Greet")]

    [InlineData("Greet{name}{now}",         "Greet")]
    [InlineData("Greet{name:s}{now:d}",     "Greet")]

    [InlineData("Greet{name},{now}",        "Greet")]
    [InlineData("Greet{name:s},{now:d}",    "Greet")]
    public void Strips_ParameterNames_From_ExtensionMethodName(string input, string expected)
    {
        var sut = new ExtensionMethodBuilder(targetType, input, input, new("Ressource1.resx", input, 42), useParametersInMethodNames: false);
        var method = sut.Build();

        var actual = method.Name.ToString();
        actual.Should().Be(expected);
    }

}
