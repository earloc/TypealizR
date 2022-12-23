using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TypealizR;

internal record struct GeneratedSourceFile(string FileName, string Content, IEnumerable<Diagnostic> Diagnostics)
{
}
