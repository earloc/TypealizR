using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TypealizR.CLI.Abstractions;
using TypealizR.CLI.Commands.CodeFirst;

namespace TypealizR.CLI;
internal class App
{
    private readonly Parser runner;
    private readonly Action<IServiceCollection>? configureServices;
    private readonly string[] args;

    public App(Action<IServiceCollection>? configureServices = null, params string[] args)
    {
        this.configureServices = configureServices;
        this.args = args;
        Console.OutputEncoding = Encoding.UTF8;

        var codeFirstCommand = new Command("code-first")
        {
            new ExportCommand()
        };

        codeFirstCommand.AddAlias("cf");

        RootCommand rootCommand =
        [
            codeFirstCommand
        ];

        runner = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseHost(_ => Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => config
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddCommandLine(args)
                )
                , builder => builder
                    .UseCommandHandler<ExportCommand, ExportCommand.Implementation>()
                    .ConfigureServices(ConfigureServices)
            )
            .Build()
        ;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        configureServices?.Invoke(services);
        services.TryAddSingleton<IStorage, FileStorage>();
    }

    public Task<int> RunAsync() => runner.InvokeAsync(args);
}
