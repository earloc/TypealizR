using Microsoft.Extensions.DependencyInjection;

namespace XUnit.Tests;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var services = new ServiceCollection();
		services.AddLocalization();
		services.AddLogging();

		var provider = services.BuildServiceProvider();

		var app = new ConsoleApp.App(provider);
		var message = app.SayHello();

		Assert.Equal("Hello Earth", message);
	}
}