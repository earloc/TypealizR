using FluentAssertions;

namespace TypealizR.SourceGenerators.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var sut = new Class1();

        sut.hello.Should().Be("world");
    }
}