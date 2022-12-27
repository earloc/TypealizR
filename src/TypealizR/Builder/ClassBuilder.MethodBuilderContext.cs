﻿using TypealizR.Diagnostics;

namespace TypealizR.Builder;
internal partial class ClassBuilder
{
	private class MethodBuilderContext
	{
		public MethodBuilderContext(MethodBuilder builder, DiagnosticsCollector diagnostics)
		{
			Builder = builder;
			Diagnostics = diagnostics;
		}

		public MethodBuilder Builder { get; }
		public DiagnosticsCollector Diagnostics { get; }
	}
}