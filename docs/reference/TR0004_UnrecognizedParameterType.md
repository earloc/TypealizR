# TR0004: UnrecognizedParameterType

## Cause
The processed Resx-file contains a key that uses a parameter with an unrecognized type-annotation

## Rule description

When extracting parameters for a method-signature, the source-generator tries to infer a better suitable parameter-type to be used when constructing the method-signature.
This is done by inspecting the parameter-token after the colon `:`, if present.

> e.g. `xyz` as ín `hello {world:xyz}`

If no parameter-annotation was found, the generator just assumes to use `object`.

When a value is found, it is matched against the following built-in data-types, in order to generate a stricter typed method-signature:

|alias   | type            | example usage (alias) | example usage (type)  |
|-------|-----------------|-----------------------|-----------------------|
| i      | int             | {count:i}             | {count:int}           |
| s      | string          | {name:s}              | {name:string}         |
| dt     | DateTime        | {date:dt}             | {date:DateTime}       |
| dto    | DateTimeOffset  | {date:dto}            | {date:DateTimeOffset} |
| d      | DateOnly        | {date:d}              | {date:DateOnly}       |
| t      | TimeOnly        | {time:t}              | {time:TimeOnly}       |

However, when the provided parameter-annotation could not be matched, the generator still falls back to using `object` as the parameter-type within the generated method-signature and emits this warning, additionaly.

## How to fix violations
Remove the type-annotation or use a valid one as shown in above table.

## When to suppress warnings
If you don´t care that your code-base has an easy-to-fix issue.

## Example of a violation

### Description
The following content of a Resx-file demonstrate, what keys produce this warning. 
### Code

```xml
<root>
  <data name="Hello {name:xyz}." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
</root>
```

## Example of how to fix

### Description
In order to fix this particular situation, just remove the type-annotation, or use one of the built-in supported ones.
### Code

```xml
<root>
  <data name="Hello {name}." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
</root>
```

or 

```xml
<root>
  <data name="Hello {name:s}." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
</root>
```

or 

```xml
<root>
  <data name="Hello {name:int}." xml:space="preserve">
    <value>Hello, {0}.</value>
  </data>
</root>
```

or similar ;)

## Related rules
