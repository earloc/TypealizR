# TYPEALIZR001010: AmbigiousRessourceKey

## Cause
The processed Resx-file contains keys that end up in possible duplicate extension-method names.

## Rule description

During generation of extension-methods, the generated method-name has to be sanitized in order to produce compilabe results. 
This can lead to ambigious names, which ultimately would prevent the overall compilation-process.
In order to prevent users of TypealizR to fall into that pit of not being able to compile anymore, TypealizR de-duplicates the generated method-name by simply appending a running number to all possible duplications of a method.
To further help users getting a more accessible code-base, this warning is emitted - which directly points to the Resx-file and the affected line.

## How to fix violations
Rename the reported ressource-key in a way that does not produce a conflict when deriving a method-name from it.

## When to suppress warnings
If you´re OK with the generated method-name, that automagically got de-duplicated ;)

## Example of a violation

### Description
The following content of a Resx-file demonstrate, what keys might end up with duplicated method-names. 
All keys would end up in the following method-name: `Hello_World()`, if this case wasn´t handled by TypealizR:
### Code

```
<root>
  <data name="Hello, World." xml:space="preserve"> <!-- would be derived to "Hello_World()" -->
    <value>Hello, World.</value>
  </data>
  <data name="Hello, World!" xml:space="preserve"> <!-- would be derived to "Hello_World()" -->
    <value>Hello, World!</value>
  </data>
  <data name="Hello, World?" xml:space="preserve"> <!-- would be derived to "Hello_World()" -->
    <value>Hello, World?</value>
  </data>
</root>
```

## Example of how to fix

### Description
In order to fix this particular situation, just re-phrase the affected keys.
### Code

```
<root>
  <data name="Hello, World." xml:space="preserve"> <!-- will be derived to "Hello_World()" -->
    <value>Hello, World.</value>
  </data>
  <data name="Shout Hello, World!" xml:space="preserve"> <!-- will be derived to "Shout_Hello_World()" -->
    <value>Hello, World!</value>
  </data>
  <data name="Ask Hello, World?" xml:space="preserve"> <!-- will be derived to "Ask_Hello_World()" -->
    <value>Hello, World?</value>
  </data>
</root>
```

## Related rules