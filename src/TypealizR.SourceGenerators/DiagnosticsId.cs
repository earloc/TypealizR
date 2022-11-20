namespace TypealizR.SourceGenerators;
internal record struct DiagnosticsEntry (string Code, string Title)
{
	internal static string LinkToDocs(DiagnosticsEntry id) => $"https://github.com/earloc/TypealizR/blob/main/docs/reference/{id.Code}_{id.Title}.md";
}
