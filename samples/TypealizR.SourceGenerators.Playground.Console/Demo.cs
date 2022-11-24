using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
namespace TypealizR.SourceGenerators.Playground.Console;
internal class Demo
{

	public Demo(IStringLocalizer<Demo> localize)
	{
		var message = localize.Hello__App("TypealizR");
		System.Console.WriteLine(message);
	}
}
