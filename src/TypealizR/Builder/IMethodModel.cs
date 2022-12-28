namespace TypealizR.Builder;

internal interface IMethodModel
{
	TypeModel ExtendedType { get; }
	string Name { get; }
	string RawRessourceName { get; }

	string ToCSharp();
}