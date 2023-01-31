using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.CLI;

namespace TypealizR.Tests.CLI.Tests;
public class ExportCommand_Tests
{
    private static string ProjectFile(string x) =>  $"../../../../{x}/{x}.csproj";

    private App CreateSUT(out InMemoryStorage storage)
    {
        var dependencies = new App.Dependencies();
        var storageMock = new InMemoryStorage();
        
        dependencies.Storage = x => storageMock;
        var app = new App();
        storage = storageMock;
        return app;
    }

    [Fact]
    public async Task Export_SingleInterface_SingleProperty_Generates_Resx()
    {
        var result = await CreateSUT(out var storage)
            .RunAsync("code-first", "export", ProjectFile("Tests.CLI.SingleInterface_SingleProperty"));

        result.Should().Be(0);
    }
}
