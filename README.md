[![.NET](https://github.com/earloc/TypealizR/actions/workflows/dotnet.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/earloc/TypealizR/badge.svg?branch=main)](https://coveralls.io/github/earloc/TypealizR?branch=main)
[![CodeQL](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml)
[![Publish](https://github.com/earloc/TypealizR/actions/workflows/publish.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR)
[![NuGet (unstable)](https://img.shields.io/nuget/vpre/TypealizR)]((https://www.nuget.org/packages/TypealizR))

# TypealizR
> The **type**d internation**aliz**e**R**

Strongly typed i18n support for the .NET - ecosystem

## usage

use this:

```csharp

@inject IStringLocalizer<HomePage> localize;
@inject AppUser user;

<h1>@localize.Title()<h1>
<h2>@localize.Welcome_back__userName(user.GivenName)<h2>

```

instead of this:


```csharp

@inject IStringLocalizer<HomePage> localize;
@inject AppUser user;

<h1>@localize["Title"]<h1>
<h2>@localize["Welcome back, {0}", user.GivenName]<h2>

```

## how it works

TypealizR parses ordinary Resx-files and generates extension-classes and -methods using `source-generators` on the fly.

![demo_typealize_translation_initial]


given the following folder-structure:

```
root/
+---/Pages/
     +---/HomePage.razor
     +---/HomePage.resx
     +---/HomePage.en-EN.resx
     +---/HomePage.de.resx
```

where `HomePage.resx` looks like this:

| key | value |
|------|-------|
| Title | Home |
|Welcome back, {userName}, this is your {visitCount:i} visit | Welcome back, {0}, this is your {1} visit to the app |
|Good bye, {userName:s} | See you later, {0} |

TypealizR emits the following class (comments, usings, etc. omitted):

```csharp

internal static class IStringLocalizerExtensions_Root_Pages_HomePage 
{
	public static string Title(
		this IStringLocalizer<Root.Pages.HomePage> that) 
		=> that["Title"];
		
	public static string Welcome_back__userName_this_is_your__visitCount__visit(
		this IStringLocalizer<Root.Pages.HomePage> that, object userName, int visitCount) 
			=> that["Welcome back, {0}, this is your {1} visit to the app", userName, visitCount];
		
	public static string Good_bye__userName(
		this IStringLocalizer<Root.Pages.HomePage> that, string userName) 
			=> that["See you later, {0}", userName];
}

```

which then can be used in favor of the lesser-typed default-syntax of IStringLocalizer&lt;T&gt;


## setup

- install via [![NuGet](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR) (latest stable) or [![NuGet (unstable)](https://img.shields.io/nuget/vpre/TypealizR)]((https://www.nuget.org/packages/TypealizR)) (latest preview)
- modify target csproj (where those precious ResX-files are ;P)
```xml
<PropertyGroup>
	<!-- Update the property to include all EmbeddedResource files -->
	<AdditionalFileItemNames>$(AdditionalFileItemNames);EmbeddedResource</AdditionalFileItemNames>
</PropertyGroup>
```
- rebuild target csproj
  > NOTE: visual-studio might need a fresh restart after installing (or updating) TypealizR in order to work as expected
- start utilizing strongly typed ressources

[demo_typealize_translation_initial]:docs/assets/demo_typealize_translation_initial.gif


## customize warnings

During code-generation, the `code-generator` might emit one of [these diagnostics](https://github.com/earloc/TypealizR/tree/main/docs/reference).

To modify the severity of each reported diagnostics, provide a `.globalconfig`-file in the root directory of the project which consumes `TypealizR`.

### samples

To ignore all diagnostics emitted by `TypealizR`, provide the following content to `.globalconfig`:
```
is_global = true
dotnet_diagnostic_TR0002_severity = hidden
dotnet_diagnostic_TR0003_severity = hidden
dotnet_diagnostic_TR0004_severity = hidden
```

To treat all diagnostics emitted by `TypealizR` as a compiler-error, supply the following contents:
```
is_global = true
dotnet_diagnostic_TR0002_severity = error
dotnet_diagnostic_TR0003_severity = error
dotnet_diagnostic_TR0004_severity = error
```

See 
  - [global-analyzerconfig](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files#global-analyzerconfig) for further details about analyzer-configs.
  - #12 for details about design-decisssions
  - #35 for implementation-details


