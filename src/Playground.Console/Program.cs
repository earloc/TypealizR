// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using My.Super.Special.Namespace;
using Playground.Console.NoCodeGen;
using Playground.Console.WithCodeGen;
using Playground.Shared;
using Playground.Shared.Groups;
using Playground.Shared.Groups.TypealizR;
using Playground.Shared.NoCodeGen;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
services.AddLogging();
services.AddLocalization();

services.AddScoped(x => x.GetRequiredService<IStringLocalizer<Ressources>>().Typealize());
var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var typealized = scope.ServiceProvider.GetRequiredService<TypealizedRessources>();

Console.WriteLine(typealized.Messages.Info.Info1);
Console.WriteLine(typealized.Messages.Info.Info2);
Console.WriteLine(typealized.Messages.Info.Info3);
Console.WriteLine(typealized.Questions.What_to_do);
Console.WriteLine(typealized.Questions.What_to_do__now(DateTime.Now));


services.AddSingleton<Greeter, Greeter>();

var customNamespace = provider.GetRequiredService<IStringLocalizer<CustomNameSpace>>();
Console.WriteLine(customNamespace.Hello());


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


var groups = provider.GetRequiredService<IStringLocalizer<Ressources>>();
Console.WriteLine(
    groups.SomeDeeplyNestedThingCalledAfterAMonster_With_the__name("Chewbacca")
);

var typealizedGroups = groups.Typealize();

Console.WriteLine(
    typealizedGroups.Some.Deeply.Nested.Thing.Called.After.A.Monster.With_the__name("Chewbacca")
);

var without_Params_In_MethodNames = provider.GetRequiredService<IStringLocalizer<Without_Params_In_MethodNames>>();

Console.WriteLine(
    without_Params_In_MethodNames.Hello("Earth")
);

Console.WriteLine(
    without_Params_In_MethodNames.Goodbye("Arthur")
);










var g = typealizedGroups;

static void SomeMethod(IStringLocalizer<Ressources> L)
{
    //use L
}

SomeMethod(g.Localizer);


Console.WriteLine(
    g.Some.Deeply.Nested.Thing.Called.After.A.Monster.It
);

Console.WriteLine(
    g.Some.Deeply.Nested.Thing.Called.After.A.Monster.With_the__name("Chewbacca")
);