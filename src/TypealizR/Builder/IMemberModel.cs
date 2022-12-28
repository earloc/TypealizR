namespace TypealizR.Builder;

internal interface IMemberModel
{
	TypeModel ExtendedType { get; }
	string Name { get; }
	string RawRessourceName { get; }

	void DeduplicateWith(int discriminator);

	string ToCSharp();
}