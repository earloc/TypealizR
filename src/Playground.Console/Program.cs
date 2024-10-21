#pragma warning disable CA1812 // Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using My.Super.Special.Namespace;
using Playground.Console.NoCodeGen;
using Playground.Console.WithCodeGen;
using Playground.Common;
using Playground.Common.Groups;
using Playground.Common.Groups.TypealizR;
using Playground.Common.NoCodeGen;
using System.Runtime.InteropServices;

const string arthur = "Arthur";
const string chewbacca = "Chewbacca";

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

var genericLocalizer = provider.GetRequiredService<IStringLocalizer>();

var customNamespace = provider.GetRequiredService<IStringLocalizer<CustomNameSpace>>();
Console.WriteLine(customNamespace.Hello());


var greeter = provider.GetRequiredService<Greeter>();

greeter.SayHello(arthur);
greeter.SayHelloPublic(arthur);

var internalLocalizable = provider.GetRequiredService<IStringLocalizer<InternalClass>>();
Console.WriteLine(internalLocalizable.Hello__name(arthur));


var publicLocalizable = provider.GetRequiredService<IStringLocalizer<PublicClass>>();
Console.WriteLine(publicLocalizable.Hello__name(arthur));


var localize = internalLocalizable;

var userName = arthur;
var today = DateOnly.FromDateTime(DateTimeOffset.Now.UtcDateTime);

localize.Hello__user__it_is__today(userName, today);


var groups = provider.GetRequiredService<IStringLocalizer<Ressources>>();
Console.WriteLine(
    groups.SomeDeeplyNestedThingCalledAfterAMonster_With_the__name(chewbacca)
);

var typealizedGroups = groups.Typealize();

Console.WriteLine(
    typealizedGroups.Some.Deeply.Nested.Thing.Called.After.A.Monster.With_the__name(chewbacca)
);

var without_Params_In_MethodNames = provider.GetRequiredService<IStringLocalizer<Without_Params_In_MethodNames>>();

Console.WriteLine(
    without_Params_In_MethodNames.Hello("Earth")
);

Console.WriteLine(
    without_Params_In_MethodNames.Goodbye(arthur)
);

#pragma warning restore CA1812 // Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible









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