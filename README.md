# TypealizR

## usage

- install via nuget
- modify target csproj
	```xml

		<PropertyGroup>
			<!-- Update the property to include all EmbeddedResource files -->
			<AdditionalFileItemNames>$(AdditionalFileItemNames);EmbeddedResource</AdditionalFileItemNames>
		</PropertyGroup>

	```

