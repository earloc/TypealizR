namespace TypealizR.SourceGenerators;
internal record struct DiagnosticsId (string Code, string Title)
{
	internal static string LinkToDocs(DiagnosticsId id) => $"https://github.com/earloc/TypealizR/blob/main/docs/reference/{id.Code}_{id.Title}.md";
}
