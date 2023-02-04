using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Hosting;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI.Abstractions;
using TypealizR.CLI.Binders;
using TypealizR.CLI.Commands.CodeFirst;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TypealizR.CLI;
internal class App
{
    private readonly Parser runner;
    private readonly Action<IServiceCollection>? configureServices;

    public App(Action<IServiceCollection>? configureServices = null)
	{
        this.configureServices = configureServices;

        Console.OutputEncoding = Encoding.UTF8;

        var codeFirstCommand = new Command("code-first")
        {
            new ExportCommand()
        };

        codeFirstCommand.AddAlias("cf");

        var rootCommand = new RootCommand()
        {
            codeFirstCommand
        };

        runner = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseHost(_ => Host.CreateDefaultBuilder(), builder =>
                builder
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

    public Task<int> RunAsync(params string[] args) => runner.InvokeAsync(args);
}
