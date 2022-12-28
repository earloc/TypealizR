﻿using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class ExtensionClassBuilder
{
	private sealed class MethodModelContext
	{
		public MethodModelContext(MethodModel model, DiagnosticsCollector diagnostics)
		{
			Model = model;
			Diagnostics = diagnostics;
		}

		public MethodModel Model { get; }
		public DiagnosticsCollector Diagnostics { get; }
	}
}
