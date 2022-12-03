// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Playground.Shared;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();
services.AddSingleton<Greeter, Greeter>();


var provider = services.BuildServiceProvider();

var greeter = provider.GetRequiredService<Greeter>();

greeter.SayHello("Arthur");

