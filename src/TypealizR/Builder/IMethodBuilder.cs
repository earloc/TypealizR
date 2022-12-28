using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal interface IMethodBuilder
{
	IMethodModel Build(TypeModel target, DiagnosticsCollector diagnostics);
}