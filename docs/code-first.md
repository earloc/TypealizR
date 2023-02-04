
# TypealizR - code-first

## getting started

- install via [![NuGet](https://img.shields.io/nuget/v/TypealizR.CodeFirst.Abstractions)](https://www.nuget.org/packages/TypealizR.CodeFirst.Abstractions)
- Author an ordinary `interface`, marked with `CodeFirstTypealizedAttribute` somewhere within your project.
- Use properties for plain `translatables`.
  > return-type needs to be `LocalizedString`
- Use methods for type-safe translation of formatted `translatables`.
  > return-type needs to be `LocalizedString`
- Utilize `structured xml comments` to provide custom default-values.

```csharp
[CodeFirstTypealized]
public interface ILocalizables
{
    LocalizedString Hello(string world);

    /// <summary>
    /// 42
    /// </summary>
    LocalizedString WhatIsTheMeaningOfLifeTheUniverseAndEverything { get; }

    /// <summary>
    /// So long, '<paramref name="user"/>'. And thx for all the fish!
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    LocalizedString Farewell(string user);

    /// <summary>
    /// <paramref name="left"/> greets <paramref name="right"/>, and <paramref name="right"/> replies: "Hi!".
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    LocalizedString Greet(string left, string right);
}
```


Based on such an interface, [TypealizR] will generate a default implementation, which easily can be dependency injected:

```csharp
void Demo(ILocalizables i18n)
{
    Console.WriteLine(i18n.Hello("Earth")); // Hello Earth
    Console.WriteLine(i18n.Farewell("Arthur")); // So long, 'Arthur'. And thx for all the fish!
    Console.WriteLine(i18n.WhatIsTheMeaningOfLifeTheUniverseAndEverything); // 42
    Console.WriteLine(i18n.Greet(right: "Zaphod", left: "Arthur")); // Arthur greets Zaphod, and Zaphod replies: "Hi!".
}
```

### synchronize resources
> not supported, yet. But will be awesome ;)