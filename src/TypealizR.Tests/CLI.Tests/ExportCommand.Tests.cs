using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI;
using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests;
public class ExportCommand_Tests
{
    private static string ProjectFile(string x) =>  $"../../../../{x}/{x}.csproj";

    [Fact]
    public async Task Export_SingleInterface_SingleProperty_Generates_Resx()
    {
        var storage = new InMemoryStorage();
        var sut = new App( services => services.AddSingleton<IStorage>(_ => storage) );
        var result = await sut
            .RunAsync("code-first", "export", ProjectFile("Playground.CodeFirst.Console"));
        result.Should().Be(0);

        storage.Files.Keys.Should().ContainMatch("*ILocalizables.resx");
        storage.Files.Keys.Should().ContainMatch("*ILocalizablesWithDefaults.resx");
    }
}
