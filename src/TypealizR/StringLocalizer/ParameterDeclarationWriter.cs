using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypealizR.StringLocalizer;
internal class ParameterDeclarationWriter
{
	private IEnumerable<ParameterModel> model;

	public ParameterDeclarationWriter(IEnumerable<ParameterModel> model)
	{
		this.model = model;
	}

	internal string ToCSharp() => model.Select(x => $"{x.Type} {x.DisplayName}").ToCommaDelimited();
}
