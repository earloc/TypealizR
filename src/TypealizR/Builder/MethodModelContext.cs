using TypealizR.Diagnostics;

namespace TypealizR.Builder;

internal sealed class MethodModelContext
{
	public MethodModelContext(IMemberModel model, DiagnosticsCollector diagnostics)
	{
		Model = model;
		Diagnostics = diagnostics;
	}

	public IMemberModel Model { get; }
	public DiagnosticsCollector Diagnostics { get; }
}
