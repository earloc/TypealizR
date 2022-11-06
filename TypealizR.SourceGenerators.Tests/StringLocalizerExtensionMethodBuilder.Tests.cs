using System;
using System.Collections.Generic;
using System.Text;
using TypealizR.Extensions.System;

namespace TypealizR.SourceGenerators
{
    public class StringLocalizerExtensionMethodBuilder_Tests
    {
        [Theory]
        [InlineData("Name", "Name")]
        [InlineData("Hello World!", "Hello_World")]
        [InlineData("Hello World.", "Hello_World")]
        [InlineData("Hello World?", "Hello_World")]
        [InlineData("Hello {0}", "Hello__0")]
        [InlineData("{0} Hello {0}", "Hello__0")]
        [InlineData("Hello {name}", "Hello__name")]
        [InlineData("Hello {name}, today is {now}", "Hello__name__today_is__now")]
        public void Ensures_Compilable_ExtensionMethodName(string input, string expected)
        {
            var sut = new StringLocalizerExtensionMethodBuilder(input, input);
            var method = sut.Build("T1");

            var actual = method.Name;
            actual.Should().Be(expected);
        }
    }
}
