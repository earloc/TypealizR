using ConsoleSTS;
using ConsoleSTS.CodeFirst;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Text;

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();
services.AddSingleton<ILocalizables, Localizables>();

var provider = services.BuildServiceProvider();
Console.OutputEncoding = Encoding.UTF8;


//resource-first
var localize = provider.GetRequiredService<IStringLocalizer<App>>();

Console.WriteLine(localize.Hello_World());
Console.WriteLine(localize.Hello_World1());
Console.WriteLine(localize.Hello_World2());

Console.WriteLine(localize.Hello__0("Arthur"));
Console.WriteLine(localize.Hello__UserName(UserName: "Arthur"));
var today = DateOnly.FromDateTime(DateTime.Now);

Console.WriteLine(localize.Hello__name__today_is__now(name: "Arthur", now: today));


//code-first
var codeFirst = provider.GetRequiredService<ILocalizables>();

Console.WriteLine(codeFirst.Hello);
Console.WriteLine(codeFirst.Greet("Arthur", "earth"));
