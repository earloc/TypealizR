using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

var services = new ServiceCollection();
services.AddLocalization();
var provider = services.BuildServiceProvider();

var localize = provider.GetRequiredService<IStringLocalizer<Program>>();
