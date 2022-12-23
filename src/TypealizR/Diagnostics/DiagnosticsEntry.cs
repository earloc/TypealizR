namespace TypealizR.Diagnostics;
internal record struct DiagnosticsEntry (DiagnosticsId Id, string Title)
{
	internal static string LinkToDocs(DiagnosticsEntry that) => $"https://github.com/earloc/TypealizR/blob/main/docs/reference/{that.Id}_{that.Title}.md";
}
