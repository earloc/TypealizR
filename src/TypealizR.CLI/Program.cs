

using System.CommandLine;
using TypealizR.CLI.Commands.CodeFirst;


var directoryOption = new Option<string>("--directory");
directoryOption.AddAlias("-d");

var rootCommand = new RootCommand();

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

await rootCommand.InvokeAsync(args);
