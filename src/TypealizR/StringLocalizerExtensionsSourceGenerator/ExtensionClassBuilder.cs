﻿using TypealizR.Core;
using TypealizR.Core.Diagnostics;

namespace TypealizR;
internal partial class ExtensionClassBuilder
{
    private readonly TypeModel markerType;
    private readonly string rootNamespace;
    private readonly bool useParametersInMethodNames;

    public ExtensionClassBuilder(TypeModel markerType, string rootNamespace, bool useParametersInMethodNames)
    {
        this.markerType = markerType;
        this.rootNamespace = rootNamespace;
        this.useParametersInMethodNames = useParametersInMethodNames;
    }

    private readonly DeduplicatingCollection<ExtensionMethodModel> methods = new();

    public ExtensionClassBuilder WithExtensionMethod(string key, string value, DiagnosticsCollector diagnostics)
    {
        var builder = new ExtensionMethodBuilder(rootNamespace, useParametersInMethodNames, markerType, key, value, diagnostics);
        var model = builder.Build();

        methods.Add(model, diagnostics);

        return this;
    }

    public ExtensionClassModel Build() => new(markerType, rootNamespace, methods.Items);
}
