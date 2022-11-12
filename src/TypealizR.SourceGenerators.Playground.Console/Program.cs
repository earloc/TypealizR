using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TypealizR.SourceGenerators.Playground.Console;

var services = new ServiceCollection();
services.AddLocalization();
var provider = services.BuildServiceProvider();

var localize = provider.GetRequiredService<IStringLocalizer<App>>();

Console.WriteLine(localize.Hello_World());