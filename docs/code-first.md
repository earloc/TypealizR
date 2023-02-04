
# TypealizR - code-first

## getting started

- install [TypealizR](https://www.nuget.org/packages/TypealizR)  via [![NuGet](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR)
- install [TypealizR.CodeFirst.Abstractions](https://www.nuget.org/packages/TypealizR.CodeFirst.Abstractions) via [![NuGet](https://img.shields.io/nuget/v/TypealizR.CodeFirst.Abstractions)](https://www.nuget.org/packages/TypealizR.CodeFirst.Abstractions)
- Author a `Typealized-Interface` which basically is an ordinary `interface`, marked with `CodeFirstTypealizedAttribute` somewhere within your project.
  - Use properties for plain `translatables`.
    > return-type needs to be `LocalizedString`
  - Use methods for type-safe translation of formatted `translatables`.
    > return-type needs to be `LocalizedString`
  - Utilize `structured xml comments` to provide custom default-values.

![TypealizedInterface](https://github.com/earloc/TypealizR/blob/main/docs/assets/demo_TypealizedInterface.png?raw=true)

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

Based on such an interface, TypealizR will generate a default implementation, which easily can be dependency injected:

```csharp
void Demo(ILocalizables i18n)
{
    Console.WriteLine(i18n.Hello("Earth")); // Hello Earth
    Console.WriteLine(i18n.Farewell("Arthur")); // So long, 'Arthur'. And thx for all the fish!
    Console.WriteLine(i18n.WhatIsTheMeaningOfLifeTheUniverseAndEverything); // 42
    Console.WriteLine(i18n.Greet(right: "Zaphod", left: "Arthur")); // Arthur greets Zaphod, and Zaphod replies: "Hi!".
}
```

## synchronize resources
- install [TypealizR.CLI](https://www.nuget.org/packages/TypealizR.CLI) (as local or global tool) via [![NuGet](https://img.shields.io/nuget/v/TypealizR.CLI)](https://www.nuget.org/packages/TypealizR.CLI)
- run it on your project
  - `dotnet tr code-first export some/path/to/a.csproj`, or alternatively
    > `dotnet tr cf ex some/path/to/a.csproj`

Will extract the following `resx`-file:

![TypealizedInterface_Resx](https://github.com/earloc/TypealizR/blob/main/docs/assets/demo_TypealizedInterface_Resx.png?raw=true)

```xml
<data name="WhatIsTheMeaningOfLifeTheUniverseAndEverything">
    <value>42</value>
  </data>
  <data name="Hello">
    <value>Hello {0}</value>
  </data>
  <data name="Farewell">
    <value>Goodbye, {0}</value>
  </data>
  <data name="Greet">
    <value>{1} greets {0}, and {0} answers: ""Hi!"".</value>
  </data>
```

