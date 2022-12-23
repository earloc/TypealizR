using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypealizR.Tests.Snapshots;
internal class ResxFile: AdditionalText
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
