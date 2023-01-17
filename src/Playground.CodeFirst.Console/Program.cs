// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TypealizR.CodeFirst.Abstractions;

Console.WriteLine("Hello, World!");

ILocalizables i18n = new Localizables();

Console.WriteLine(i18n.Hello(world:"Earth"));

[TypealizR]
interface ILocalizables
{
    /// <summary>
    /// Hello <paramref name="world"/>
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    string Hello(string world);
}

partial class Localizables : ILocalizables
{
    public string Hello(string world) => $"Hello {world}";
}
