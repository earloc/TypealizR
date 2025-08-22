using Microsoft.CodeAnalysis;
using TypealizR.Core;

namespace TypealizR.Tests;

public class ExtensionMethodBuilder_Tests
{
    private static readonly TypeModel targetType = new("Name.Space", "TypeName", [], Accessibility.Internal);

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
    [InlineData("Hello {name:int@x}, today is {now:int@x}", "Hello__name__today_is__now")]
    [InlineData("Hello {name:i@x/y}", "Hello__name")]

    public void Ensures_Compilable_ExtensionMethodName(string input, string expected)
    {
        var sut = new ExtensionMethodBuilder("Some.Name.Space", true, targetType, input, input, new("Ressource1.resx", input, 42));
        var method = sut.Build();

        var actual = method.Name.ToString();
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("Greet", "Greet")]
    [InlineData("Greet {0}", "Greet")]
    [InlineData("{0} Greet {0}", "Greet")]
    [InlineData("Greet {name}", "Greet")]
    [InlineData("Greet {name:s}", "Greet")]
    [InlineData("Greet{name}", "Greet")]
    [InlineData("Greet{name:s}", "Greet")]
    [InlineData("Greet{name} ", "Greet")]
    [InlineData("Greet{name:s} ", "Greet")]

    [InlineData("Greet {name} {now}", "Greet")]
    [InlineData("Greet {name:s} {now:d}", "Greet")]

    [InlineData("Greet {name}, {now}", "Greet")]
    [InlineData("Greet {name:s}, {now:d}", "Greet")]

    [InlineData("Greet {name}{now}", "Greet")]
    [InlineData("Greet {name:s}{now:d}", "Greet")]

    [InlineData("Greet{name}{now}", "Greet")]
    [InlineData("Greet{name:s}{now:d}", "Greet")]

    [InlineData("Greet{name},{now}", "Greet")]
    [InlineData("Greet{name:s},{now:d}", "Greet")]

    [InlineData("Hello {name}", "Hello")]
    [InlineData("Hello {name:int}", "Hello")]
    [InlineData("Hello {name:int@ex}", "Hello")]
    public void Strips_ParameterNames_From_ExtensionMethodName(string input, string expected)
    {
        var sut = new ExtensionMethodBuilder("Some.Name.Space", false, targetType, input, input, new("Ressource1.resx", input, 42));
        var method = sut.Build();

        var actual = method.Name.ToString();
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData("Hello", "Hello")]
    [InlineData("\"Hello\"", "\\\"Hello\\\"")]
    public void Escapes_DoubleQuotes_InKey(string input, string expected)
    {
        var sut = new ExtensionMethodBuilder("Some.Name.Space", false, targetType, input, input, new("Ressource1.resx", input, 42));
        var method = sut.Build();

        var actual = method.ResourceKey.ToString();
        actual.ShouldBe(expected);
    }

}
