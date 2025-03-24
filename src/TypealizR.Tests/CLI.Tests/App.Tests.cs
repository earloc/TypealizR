using TypealizR.CLI;

namespace TypealizR.Tests.CLI.Tests;
public class App_Tests
{

    [Fact]
    public void CanCreate_Instance_Without_Custom_ServiceConfiguration()
    {
        var sut = new App();
        sut.ShouldNotBeNull();
    }
}
