// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TypealizR.CodeFirst.Abstractions;

var services = new ServiceCollection();
var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();


var i18n = scope.ServiceProvider.GetRequiredService<ILocalizables>();
Console.WriteLine(i18n.Hello("Earth"));


[CodeFirstTypealized]
interface ILocalizables
{
    ///<summary>
    /// Hello '<paramref name="world"/>'
    ///</summary>
    /// <param name="world">the name of the world to be greeted</param>
    /// <returns></returns>
    LocalizedString Hello(string world);
}

//partial class Localizables : ILocalizables
//{
//    public LocalizedString Hello(string world) => localizer["Hello '{0}'", world];
//}
