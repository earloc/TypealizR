using System;
using Microsoft.Extensions.Localization;

namespace Playground.Shared;
public class Greeter
{
    private readonly IStringLocalizer<Greetings> localize;
    private readonly IStringLocalizer<Duplicate.Greetings> duplicates;
    private readonly IStringLocalizer<Duplicate.PublicClass> publicDuplicate;

    public Greeter(IStringLocalizer<Greetings> localize, IStringLocalizer<Duplicate.Greetings> duplicates, IStringLocalizer<Duplicate.PublicClass> publicDuplicate)
    {
        this.localize = localize;
        this.duplicates = duplicates;
        this.publicDuplicate = publicDuplicate;
    }

    public void SayHello(string name) => Console.WriteLine(localize.Hello__name(name));

    public void SayHelloPublic(string name) => Console.WriteLine(publicDuplicate.Hello__name(name));


    public void GoodMorning(string name) => Console.WriteLine(duplicates.GoodMorning__name(name));

}
