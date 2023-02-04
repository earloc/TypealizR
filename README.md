
[![build](https://github.com/earloc/TypealizR/actions/workflows/build.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/earloc/TypealizR/badge.svg?branch=main&q=1)](https://coveralls.io/github/earloc/TypealizR?branch=main)
[![CodeQL](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml)
[![Publish](https://github.com/earloc/TypealizR/actions/workflows/publish.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/publish.yml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=bugs)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=earloc_TypealizR&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=earloc_TypealizR)

[![Samples STS](https://github.com/earloc/TypealizR/actions/workflows/samples_sts.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/samples_sts.yml)
[![Samples LTS](https://github.com/earloc/TypealizR/actions/workflows/samples_lts.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/samples_lts.yml)

[![NuGet](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR)
![Nuget](https://img.shields.io/nuget/dt/TypealizR)
[![NuGet (preview)](https://img.shields.io/nuget/vpre/TypealizR)]((https://www.nuget.org/packages/TypealizR))

# TypealizR
> The **type**d internation**aliz**e**R**

Statically typed i18n support for the .NET - ecosystem

## [resource-first](https://github.com/earloc/TypealizR/blob/main/docs/resource-first.md)

### ✔️ **DO** this:

```csharp

@inject IStringLocalizer<HomePage> localize;
@inject AppUser user;

<h1>@localize.Title()<h1>
<h2>@localize.Welcome_back__userName(user.GivenName)<h2>

```

![demo_typealize_translation_initial](https://github.com/earloc/TypealizR/blob/main/docs/assets/demo_typealize_translation_initial.gif?raw=true)

### ❌ **DON´T** do that:

```csharp

@inject IStringLocalizer<HomePage> localize;
@inject AppUser user;

<h1>@localize["Title"]<h1>
<h2>@localize["Welcome back, {0}", user.GivenName]<h2>

```

See [resource-first](https://github.com/earloc/TypealizR/blob/main/docs/resource-first.md) for more details

## [code-first](https://github.com/earloc/TypealizR/blob/main/docs/code-first.md)

### ✔️ **DO** this:

```csharp
void Demo(ILocalizables i18n)
{
    Console.WriteLine(i18n.Hello("Earth")); // Hello Earth
    Console.WriteLine(i18n.Farewell("Arthur")); // So long, 'Arthur'. And thx for all the fish!
    Console.WriteLine(i18n.WhatIsTheMeaningOfLifeTheUniverseAndEverything); // 42
    Console.WriteLine(i18n.Greet(right: "Zaphod", left: "Arthur")); // Arthur greets Zaphod, and Zaphod replies: "Hi!".
}

```
### ❌ **DON´T** do that:
```csharp 
void Demo(IStringLocalizer i18n)
{
    Console.WriteLine(i18n["Hello", "Earth"]); // Hello Earth
    Console.WriteLine(i18n["Farewell", "Arthur"]); // So long, 'Arthur'. And thx for all the fish!
    Console.WriteLine(i18n["WhatIsTheMeaningOfLifeTheUniverseAndEverything"]; // 42
    Console.WriteLine(i18n["Greet", "Arthur", "Zaphod")); // Arthur greets Zaphod, and Zaphod replies: "Hi!".
}
```

See [code-first](https://github.com/earloc/TypealizR/blob/main/docs/code-first.md) for more details.