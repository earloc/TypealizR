// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Playground.Console.NoCodeGen;
using Playground.Shared;
using Playground.Shared.NoCodeGen;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();
services.AddSingleton<Greeter, Greeter>();

var provider = services.BuildServiceProvider();

var greeter = provider.GetRequiredService<Greeter>();

greeter.SayHello("Arthur");

var internalLocalizable = provider.GetRequiredService<IStringLocalizer<Internal>>();
Console.WriteLine(internalLocalizable.Hello__name("Arthur"));


var publicLocalizable = provider.GetRequiredService<IStringLocalizer<Public>>();
Console.WriteLine(publicLocalizable.Hello__name("Arthur"));