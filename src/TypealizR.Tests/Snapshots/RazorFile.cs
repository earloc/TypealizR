using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypealizR.Tests.Snapshots;
internal sealed class AdditionalTextFile : AdditionalText
{
    private readonly string text;

    public override string Path { get; }

    public AdditionalTextFile(string path, string text)
    {
        Path = path;
        this.text = text;
    }

    public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken()) => SourceText.From(text);
}
