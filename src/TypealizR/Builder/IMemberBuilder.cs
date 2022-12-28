using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal interface IMemberBuilder
{
	IMemberModel Build(TypeModel target, DiagnosticsCollector diagnostics);
}