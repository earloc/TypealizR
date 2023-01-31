using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypealizR.CLI.Commands.CodeFirst;

namespace TypealizR.CLI;
internal class App
{
    RootCommand rootCommand = new ();
    public App()
	{
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
            (directory) => ExportCommand.Handle(directory, CancellationToken.None),
            directoryOption
        );

        codeFirstCommand.AddCommand(exportCommand);
    }

    public Task<int> RunAsync(params string[] args) => rootCommand.InvokeAsync(args);
}
