using Microsoft.Extensions.DependencyInjection;
using Playground.CodeFirst.Console;

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();

services.AddScoped<ILocalizables, Localizables>();
var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var i18n = scope.ServiceProvider.GetRequiredService<ILocalizables>();
//Console.WriteLine(i18n.Goodbye("Arthur"));
Demo(i18n);










static void Demo(ILocalizables i18n)
{
    Console.WriteLine(i18n.Hello("Earth")); // Hello Earth
    Console.WriteLine(i18n.Farewell("Arthur")); // Farewell Arthur
    Console.WriteLine(i18n.WhatIsTheMeaningOfLifeTheUniverseAndEverything); // WhatIsTheMeaningOfLifeTheUniverseAndEverything
    Console.WriteLine(i18n.Greet(left: "Zaphod", right: "Arthur")); // Greet Zaphod Arthur
    Console.WriteLine(i18n.Goodbye("Arthur"));
}