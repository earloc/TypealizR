namespace TypealizR.Diagnostics;
public sealed class DiagnosticsEntry
{
    public DiagnosticsEntry(DiagnosticsId id, string title)
    {
        Id = id;
        Title = title;
    }

    public DiagnosticsId Id { get; }
    public string Title { get; }

    internal static string LinkToDocs(DiagnosticsEntry that) => $"https://github.com/earloc/TypealizR/blob/main/docs/reference/{that.Id}_{that.Title}.md";
}
