using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Playground.Shared;
public class Greeter
{
	private readonly IStringLocalizer<Greetings> localize;
	private readonly IStringLocalizer<Duplicate.Greetings> duplicates;

	public Greeter(IStringLocalizer<Greetings> localize, IStringLocalizer<Duplicate.Greetings> duplicates)
	{
		this.localize = localize;
		this.duplicates = duplicates;
	}

	public void SayHello(string name) => Console.WriteLine(localize.Hello__name(name));

	public void GoodMorning(string name) => Console.WriteLine(duplicates.GoodMorning__name(name));

}
