using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypealizR.CLI.Abstractions;
using TypealizR.CLI.Binders;
using TypealizR.CLI.Commands.CodeFirst;

namespace TypealizR.CLI;
internal class App
{
    public class Dependencies
    {
        public Func<IServiceProvider, IStorage> Storage = x => new FileStorage();
    }

    readonly RootCommand rootCommand = new ();
    public App(Dependencies? dependencies = null)
	{
        dependencies ??= new();

        Console.OutputEncoding = Encoding.UTF8;

        var directoryOption = new Option<string>("--directory");
        directoryOption.AddAlias("-d");
        directoryOption.SetDefaultValueFactory(() => Directory.GetCurrentDirectory());

        var codeFirstCommand = new Command("code-first");
        codeFirstCommand.AddAlias("cf");

        rootCommand.AddCommand(codeFirstCommand);

        var exportCommand = new Command("export");
        exportCommand.AddAlias("ex");
        exportCommand.AddOption(directoryOption);
        exportCommand.SetHandler(
            (directory, storage) => ExportCommand.Handle(directory, storage, CancellationToken.None),
            directoryOption, new FuncBinder<IStorage>(dependencies.Storage)
        );

        codeFirstCommand.AddCommand(exportCommand);
    }

    public Task<int> RunAsync(params string[] args) => rootCommand.InvokeAsync(args);
}
