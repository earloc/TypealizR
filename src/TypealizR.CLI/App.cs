using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        var projectArgument = new Argument<FileInfo>("--project");

        projectArgument.SetDefaultValueFactory(
            () => Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault()
        );

        var codeFirstCommand = new Command("code-first");
        codeFirstCommand.AddAlias("cf");

        rootCommand.AddCommand(codeFirstCommand);

        var exportCommand = new Command("export");
        exportCommand.AddAlias("ex");
        exportCommand.AddArgument(projectArgument);
        exportCommand.SetHandler(
            (project, storage) => ExportCommand.Handle(project, storage, CancellationToken.None),
            projectArgument, new FuncBinder<IStorage>(dependencies.Storage)
        );

        codeFirstCommand.AddCommand(exportCommand);
    }

    public Task<int> RunAsync(params string[] args) => rootCommand.InvokeAsync(args);
}
