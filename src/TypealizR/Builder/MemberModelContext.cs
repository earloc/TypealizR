using TypealizR.Diagnostics;

namespace TypealizR.Builder;

internal sealed class MemberModelContext
{
	public MemberModelContext(IMemberModel model, DiagnosticsCollector diagnostics)
	{
		Model = model;
		Diagnostics = diagnostics;
	}

	public IMemberModel Model { get; }
	public DiagnosticsCollector Diagnostics { get; }
}
