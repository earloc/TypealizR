using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class ExtensionClassBuilder
{
	private sealed class MethodBuilderContext
	{
		public MethodBuilderContext(ExtensionMethodBuilder builder, DiagnosticsCollector diagnostics)
		{
			Builder = builder;
			Diagnostics = diagnostics;
		}

		public ExtensionMethodBuilder Builder { get; }
		public DiagnosticsCollector Diagnostics { get; }
	}
}
