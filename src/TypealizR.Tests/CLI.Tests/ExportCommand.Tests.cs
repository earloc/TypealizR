using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI;
using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests;
public class ExportCommand_Tests
{
    private static string ProjectFile(string x) => $"../../../../{x}/{x}.csproj";

    [Fact]
    public async Task Export_Generates_ResxFiles()
    {
        var storage = new InMemoryStorage();
        var sut = new App(
            services => services.AddSingleton<IStorage>(_ => storage),
            "code-first", "export", ProjectFile("Playground.CodeFirst.Console")
        );
        var result = await sut
            .RunAsync();
        result.ShouldBe(0);

        storage.Files.Keys.ShouldContain(x => x.EndsWith("ILocalizables.resx"));
        storage.Files.Keys.ShouldContain(x => x.EndsWith("ILocalizablesWithDefaults.resx"));
        storage.Files.Keys.ShouldContain(x => x.EndsWith("Some+Inner+ISampleInnerface.resx"));
    }
}
