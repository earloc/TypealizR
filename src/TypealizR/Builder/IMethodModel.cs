namespace TypealizR.Builder;

internal interface IMethodModel
{
	TypeModel ExtendedType { get; }
	string Name { get; }
	string RawRessourceName { get; }

	void DeduplicateWith(int discriminator);

	string ToCSharp();
}