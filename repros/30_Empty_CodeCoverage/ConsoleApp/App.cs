using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ConsoleApp;
internal class App
{
	private readonly string message;
	public App(IServiceProvider servceProvider)
	{
		var localize = servceProvider.GetRequiredService<IStringLocalizer<Resource1>>();
		message = localize["Hello {World}", "Earth"];
	}

	public string SayHello()
	{
		Console.WriteLine(message);
		return message;
	}
}
