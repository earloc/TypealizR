
[![build](https://github.com/earloc/TypealizR/actions/workflows/build.yml/badge.svg)](https://github.com/earloc/TypealizR/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/earloc/TypealizR/badge.svg?branch=main)](https://coveralls.io/github/earloc/TypealizR?branch=main)
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
[![NuGet (preview)](https://img.shields.io/nuget/vpre/TypealizR)]((https://www.nuget.org/packages/TypealizR))
# TypealizR
> The **type**d internation**aliz**e**R**

Strongly typed i18n support for the .NET - ecosystem

## usage

### **DO** this:

```csharp

@inject IStringLocalizer<HomePage> localize;
@inject AppUser user;

<h1>@localize.Title()<h1>
<h2>@localize.Welcome_back__userName(user.GivenName)<h2>

```

### **DON´T** do that:


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

## [type-annotations] ftw
TypealizR assists in spotting translations where specified arguments may mismatch by type / order.

### **DO** this:

Consider the following call, which might have been wrong right from the start or just became wrong over time.

When applying [type-annotations] to the parameters, these kind of bugs can be prevented and addressed at compile-time when combined with `TypealizR`s generated extension methods!

> some.resx
> 
>	```xml
>	<data name="Hello {user:s}, it is {today:d}" xml:space="preserve">
>		<value>Hello {0}, today is {1}</value>
>	</data>
>	```

> somecode.cs
> 	```csharp
> 	var userName = "Arthur";
> 	var today = DateOnly.FromDateTime(DateTimeOffset.Now.UtcDateTime);
> 
> 	localize.Hello__user__it_is__today(today, userName); 
>    // wrong ordering, which would result in the translated string "Hello 2022-01-01, today is Arthur"
>
>    // equivalent of localize["Hello {user:s}, it is {today:d}", today, userName]; 
> ```

With applied [type-annotations], this will generate tho following compile-time errors:
> - CS1503	Argument 2: cannot convert from 'System.DateOnly' to 'string'
> - CS1503	Argument 3: cannot convert from 'string' to 'System.DateOnly'


![demo_typed_parameters](docs/assets/demo_typed_parameters.gif)

### **DON´T** do that:
There's no way the default usage of `IStringLocalizer` would discover such things this early in the dev-cycle!

> some.resx
> 
>	```xml
>	<data name="Hello {user}, it is {today}" xml:space="preserve">
>		<value>Hello {0}, today is {1}</value>
>	</data>
>	```

> somecode.cs
> 	```csharp
> 	var userName = "Arthur";
> 	var today = DateOnly.FromDateTime(DateTimeOffset.Now.UtcDateTime);
> 
> 	localize["Hello {user}, it is {today}", today, userName]; 
> 	// wrong parameter-ordering, which would result in the translated string "Hello 2022-01-01, today is Arthur"
> ```


## setup

- install via [![NuGet](https://img.shields.io/nuget/v/TypealizR)](https://www.nuget.org/packages/TypealizR)
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

# extensibilty

## customize string formatting

Starting with [v0.6], TypealizR supports customizing the internal usage of `string.Format()`, which should enable developers to implement [#16] with the technology / library / approach of their choice - Leaving TypelaziR un-opinionated about the actual approach to achieve this.

To customize the formatting, just drop a custom implementation of `TypealizR_StringFormatter` anywhere in the project. The types `namespace` MUST match the project´s root-namespace.

### example
Given the root-namespace `TypealizR.Ockz` for the project consuming TypealizR, this partial-class declaration should be enough:

```csharp
namespace TypealizR.Ockz;
internal static partial class TypealizR_StringFormatter
{
	internal static partial string Format(string s, object[] args) => 
		new(string.Format(s, args).Reverse().ToArray());
}
```

With this implementation, every localized string would be reversed. (Even if that doesn´t make any sense ;P)


# configuration
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
- [global-analyzerconfig] for further details about analyzer-configs.
- [#12] for details about design-decisssions
- [#35] for implementation-details


[v0.6]:https://github.com/earloc/TypealizR/milestone/1

[#12]:https://github.com/earloc/TypealizR/issues/12
[#16]:https://github.com/earloc/TypealizR/issues/16
[#35]:https://github.com/earloc/TypealizR/pull/35

[global-analyzerconfig]:https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files#global-analyzerconfig

[type-annotations]:docs/reference/TR0004_UnrecognizedParameterType.md
