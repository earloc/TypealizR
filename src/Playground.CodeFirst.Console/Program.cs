// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TypealizR.CodeFirst.Console;

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();

services.AddScoped<ILocalizables, Localizables>();
var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var i18n = scope.ServiceProvider.GetRequiredService<ILocalizables>();

Demo(i18n);

void Demo(ILocalizables i18n)
{
    Console.WriteLine(i18n.Hello("Earth")); // Hello Earth
    Console.WriteLine(i18n.Farewell("Arthur")); // Farewell Arthur
    Console.WriteLine(i18n.WhatIsTheMeaningOfLifeTheUniverseAndEverything); // WhatIsTheMeaningOfLifeTheUniverseAndEverything
    Console.WriteLine(i18n.Greet(left: "Zaphod", right: "Arthur")); // Greet Zaphod Arthur
}