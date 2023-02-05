using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.CLI;

namespace TypealizR.Tests.CLI.Tests;
public class App_Tests
{

    [Fact]
    public void CanCreate_Instance_Without_Custom_ServiceConfiguration()
    {
        var sut = new App();
        sut.Should().NotBeNull();
    }
}
