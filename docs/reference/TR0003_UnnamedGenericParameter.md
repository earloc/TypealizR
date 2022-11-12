# TR0003: UnnamedGenericParameter

## Cause
The processed Resx-file contains a key that uses a generic format-parameter like '{0}', '{1}', etc.

## Rule description

During generation of extension-methods, the generated parameter-names are read from within the ressource-key to derive parameter-names for the methods to be generated.
When a key contains standard format-parameter values, these need to be prepended with and underscore `_` in order to get a compilable result. 
Despite this little annoyance, methods beeing generated are not very self-explanatory when being invoked

is hard to read and understand.

To guide users to provide a more meaningful name for a parameter, this warning is emitted.

## How to fix violations
Rename the reported parameter to a more meaningful name.

## When to suppress warnings
If you´re OK with lots of 0´s and 1´s, etc in the generated method- and parameter-names ;)

## Example of a violation

### Description
The following content of a Resx-file demonstrate, what keys produce this warning. 
### Code

```xml
<root>
  <data name="Hello {0}." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
  <data name="Hello '{0}', today is '{1}'." xml:space="preserve">
    <value>Hello '{0}', today is '{1}'</value>
  </data>
</root>
```

which would produce this extension-methods:

```csharp

intertnal static LocalizabelString Hello__0(this IStringLocalizer<App> that, object _0) => that["Hello {0}.", _0];
intertnal static LocalizabelString Hello__0_today_is__1(this IStringLocalizer<App> that, object _0, object _1) => that["Hello '{0}', today is '{1}'.", _0, _1];

```

## Example of how to fix

### Description
In order to fix this particular situation, just re-name the affected parameters to a more meaningful name.
### Code

```xml
<root>
  <data name="Hello '{userName}'." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
  <data name="Hello '{userName}', today is '{now}'." xml:space="preserve">
    <value>Hello '{0}', today is '{1}'</value>
  </data>
</root>
```

which would produce this extension-methods:

```csharp

intertnal static LocalizabelString Hello__userName(this IStringLocalizer<App> that, object userName) => that["Hello '{userName}'.", userName];
intertnal static LocalizabelString Hello__0_today_is__1(this IStringLocalizer<App> that, object userName, object now) => that["Hello '{userName}', today is '{now}'.", userName, now];

```

## Related rules