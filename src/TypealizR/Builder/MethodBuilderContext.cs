using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal sealed class MethodBuilderContext<TBuilder> where TBuilder : IMethodBuilder
{
	public MethodBuilderContext(TBuilder builder, DiagnosticsCollector diagnostics)
	{
		Builder = builder;
		Diagnostics = diagnostics;
	}

	public TBuilder Builder { get; }
	public DiagnosticsCollector Diagnostics { get; }
}
