[![.NET](https://github.com/earloc/TypealizR/actions/workflows/dotnet.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/codeql.yml)
[![Publish](https://github.com/earloc/TypealizR/actions/workflows/nuget.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/nuget.yml)
[![Nuget](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR)
[![Nuget (unstable)](https://img.shields.io/nuget/vpre/TypealizR)]((https://www.nuget.org/packages/TypealizR))

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

## setup

- install via nuget
  `dotnet package add TypealizR`
- modify target csproj
```xml

	<PropertyGroup>
		<!-- Update the property to include all EmbeddedResource files -->
		<AdditionalFileItemNames>$(AdditionalFileItemNames);EmbeddedResource</AdditionalFileItemNames>
	</PropertyGroup>

```
- rebuild target csproj
- start utilizing strongly typed ressources

## how it works

TypealizR parses ordinary Resx-files and generates extension-classes and -methods using `source-generators` on thy fly.

given the following folder-structure:

```
/projectDir
  |
  /Pages
    |
	/HomePage.razor
	/HomePage.resx
	/HomePage.en-EN.resx
	/HomePage.de.resx
```

where `HomePage.resx` looks like this:

| key | value |
|------|-------|
| Title | Home |
|Welcome back, {userName} | Welcome back, {0} |

TypealizR's source-generator emits the following class (comments, usings, etc. omitted):

```csharp

internal static class IStringLocalizerExtensions_HomePage 
{
	public static string Title(this IStringLocalizer<HomePage> that) => that["Title"];
	public static string Welcome_back__userName(this IStringLocalizer<HomePage> that, object userName) => that["Welcome back, {0}", userName];
}

```

which then can be used in favor of the lesser-typed default-syntax of IStringLocalizer&lt;T&gt;