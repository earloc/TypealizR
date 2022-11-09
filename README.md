# TypealizR
The **type**d internation**aliz**e**R*

Strongly typed internationalization support for the dotnet eco-system.


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
