using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI;
using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests;
public class ExportCommand_Tests
{
    private static string ProjectFile(string x) => $"../../../../{x}/{x}.csproj";

    [Theory]
    [InlineData("ILocalizables.resx")]
    [InlineData("ILocalizablesWithDefaults.resx")]
    [InlineData("Some+Inner+ISampleInnerface.resx")]
    public async Task Export_Generates_ResxFiles(string fileName)
    {
        var storage = new InMemoryStorage();
        var sut = new App(
            services => services.AddSingleton<IStorage>(_ => storage),
            "code-first", "export", ProjectFile("Playground.CodeFirst.Console")
        );
        var result = await sut
            .RunAsync();
        result.ShouldBe(0);

        var file = storage.Files.First(x => x.Key.EndsWith(fileName, StringComparison.InvariantCulture));
        await Verify(file.Value).UseParameters(fileName);
    }
}
