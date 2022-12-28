using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal sealed class MethodBuilderContext
{
	public MethodBuilderContext(IMethodBuilder builder, DiagnosticsCollector diagnostics)
	{
		Builder = builder;
		Diagnostics = diagnostics;
	}

	public IMethodBuilder Builder { get; }
	public DiagnosticsCollector Diagnostics { get; }
}
