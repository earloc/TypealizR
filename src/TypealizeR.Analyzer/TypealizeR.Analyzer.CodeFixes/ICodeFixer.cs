using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Document = Microsoft.CodeAnalysis.Document;

namespace TypealizeR.Analyzers;

internal interface ICodeFixer
{
    Task<Document> CreateChangedDocumentAsync(Document source, CancellationToken cancellationToken);
}

internal interface ICodeFixer<T> : ICodeFixer where T : SyntaxNode
{
    T Root { get; }
}
