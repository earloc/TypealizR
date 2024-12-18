﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Document = Microsoft.CodeAnalysis.Document;

namespace TypealizR.Analyzers;

internal interface ICodeFixer
{
    Task<Document> CreateChangedDocumentAsync(Document source, CancellationToken cancellationToken);
}

internal interface ICodeFixer<out T> : ICodeFixer where T : SyntaxNode
{
    T Root { get; }
}
