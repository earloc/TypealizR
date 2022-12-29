// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Playground.Console.NoCodeGen;
using Playground.Shared;
using Playground.Shared.Groups;
using Playground.Shared.NoCodeGen;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();

services.AddSingleton<Greeter, Greeter>();

var provider = services.BuildServiceProvider();

var greeter = provider.GetRequiredService<Greeter>();

greeter.SayHello("Arthur");
greeter.SayHelloPublic("Arthur");

var internalLocalizable = provider.GetRequiredService<IStringLocalizer<Internal>>();
Console.WriteLine(internalLocalizable.Hello__name("Arthur"));


var publicLocalizable = provider.GetRequiredService<IStringLocalizer<Public>>();
Console.WriteLine(publicLocalizable.Hello__name("Arthur"));


var localize = internalLocalizable;

var userName = "Arthur";
var today = DateOnly.FromDateTime(DateTimeOffset.Now.UtcDateTime);

localize.Hello__user__it_is__today(userName, today);

var groups = provider.GetRequiredService<IStringLocalizer<Groups>>();

var g = new Playground.Shared.Groups.TypealizR.StringTypealizR_Playground_Shared_Groups_Groups(groups);

Console.WriteLine(
	g.Some.Deeply.Nested.Thing.Called.After.A.Monster.It
);

Console.WriteLine(
	g.Some.Deeply.Nested.Thing.Called.After.A.Monster.With_the__name("Chewbacca")
);

