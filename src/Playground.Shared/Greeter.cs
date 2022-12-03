using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace Playground.Shared;
public class Greeter
{
	private readonly IStringLocalizer<Greetings> localize;

	public Greeter(IStringLocalizer<Greetings> localize)
	{
		this.localize = localize;
	}

	public void SayHello(string name) => Console.WriteLine(localize.Hello__name(name));
}
