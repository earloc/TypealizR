using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TypealizR.SourceGenerators.Tests;
internal class XmlLineInfo : IXmlLineInfo
{
	public int LineNumber { get; init; }

	public int LinePosition { get; init; }

	public bool HasLineInfo() => true;
}
