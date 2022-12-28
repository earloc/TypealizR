using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class ExtensionClassBuilder
{
	private sealed class MethodModelContext
	{
		public MethodModelContext(ExtensionMethodModel model, DiagnosticsCollector diagnostics)
		{
			Model = model;
			Diagnostics = diagnostics;
		}

		public ExtensionMethodModel Model { get; }
		public DiagnosticsCollector Diagnostics { get; }
	}
}
