using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypealizR.CLI;

namespace TypealizR.Tests.CLI.Tests;
public class ExportCommand_Tests
{
    private static string ProjectDir(string relativePath) => Path.Combine("../../../../", relativePath);

    [Fact]
    public async Task Export_SingleInterface_SingleProperty_Generates_Resx()
    {
        var dependencies = new App.Dependencies();
        var storage = new InMemoryStorage();
        dependencies.Storage = x => storage;

        var app = new App(dependencies);

        var projectDir = ProjectDir("Tests.CLI.SingleInterface_SingleProperty");

        await app.RunAsync("code-first", "export", "-d", projectDir);

        throw new NotImplementedException();
    }
}
