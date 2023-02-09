using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypealizR.Tests.Snapshots;
internal sealed class ResxFile : AdditionalText
{
    private readonly string text;

    public override string Path { get; }

    public ResxFile(string path)
    {
        Path = path;
        text = File.ReadAllText(path);
    }

    public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken())
    {
        return SourceText.From(text);
    }
}
