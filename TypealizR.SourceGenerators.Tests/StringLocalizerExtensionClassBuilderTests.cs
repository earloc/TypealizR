using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.SourceGenerators.StringLocalizer;

namespace TypealizR.SourceGenerators.Tests;
public class StringLocalizerExtensionClassBuilderTests
{
    [Fact]
    public void Simple_Method_Can_Be_Generated ()
    {
        var sut = new StringLocalizerExtensionClassBuilder();

        sut.WithMethodFor("SomeKey", "SomeValue");

        var classInfo = sut.Build(new("Name.Space", "TypeName"));

        classInfo.Methods.Should().HaveCount(1);

        var method = classInfo.Methods.First();


        var expected = new
        {
            Name = "SomeKey",
            Signature = "(this IStringLocalizer<Name.Space.TypeName> that)",
            Body = @"that[""SomeKey""]"
        };

        var actual = new
        {
            method.Name,
            method.Signature,
            method.Body
        };

        actual.Should().BeEquivalentTo(expected);
    }
}
