using TypealizR.Diagnostics;

namespace TypealizR.Builder;

internal sealed class MethodModelContext
{
	public MethodModelContext(IMethodModel model, DiagnosticsCollector diagnostics)
	{
		Model = model;
		Diagnostics = diagnostics;
	}

	public IMethodModel Model { get; }
	public DiagnosticsCollector Diagnostics { get; }
}
