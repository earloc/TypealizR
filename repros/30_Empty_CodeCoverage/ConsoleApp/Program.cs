// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
[assembly:InternalsVisibleTo("XUnit.Tests")]

var services = new ServiceCollection();
services.AddLocalization();
services.AddLogging();
var provider = services.BuildServiceProvider();

var app = new App(provider);

app.SayHello();