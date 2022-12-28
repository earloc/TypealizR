using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TypealizR.Core;

public class GeneratedSourceFile
{
	public GeneratedSourceFile(string fileName, string content, IEnumerable<Diagnostic> diagnostics)
	{
		FileName = fileName;
		Content = content;
		Diagnostics = diagnostics;
	}

	public string FileName { get; }
	public string Content { get; }
	public IEnumerable<Diagnostic> Diagnostics { get; }
}
