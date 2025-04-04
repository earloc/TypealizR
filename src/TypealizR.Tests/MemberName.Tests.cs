using TypealizR.Core;

namespace TypealizR.Tests;
internal class MemberName_Tests
{

    [Theory]
    [InlineData("Hello", true)]
    [InlineData("class", false)]
    [InlineData("while", false)]
    [InlineData("for", false)]
    [InlineData("if", false)]
    public void CanExecute_IsValidMethodName(string input, bool expected)
    {
        var sut = new MemberName(input);

        var actual = sut.IsValidMethodName();
        actual.ShouldBe(expected);
    }
}
