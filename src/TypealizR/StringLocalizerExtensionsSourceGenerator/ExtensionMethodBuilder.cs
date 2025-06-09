using TypealizR.Core;
using TypealizR.Core.Diagnostics;

namespace TypealizR;
internal class ExtensionMethodBuilder
{
    private readonly TypeModel markerType;
    private readonly string key;
    private readonly string value;
    private readonly DiagnosticsCollector diagnostics;
    private readonly string rootNameSpace;
    private readonly bool useParametersInMethodNames;
    private readonly ParameterBuilder parameterBuilder;

    public ExtensionMethodBuilder(string rootNameSpace, bool useParametersInMethodNames, TypeModel markerType, string key, string value, DiagnosticsCollector diagnostics)
    {
        this.rootNameSpace = rootNameSpace;
        this.useParametersInMethodNames = useParametersInMethodNames;
        this.markerType = markerType;
        this.key = key;
        this.value = value;
        this.diagnostics = diagnostics;
        parameterBuilder = new(key);
    }

    public ExtensionMethodModel Build()
    {
        var parameters = parameterBuilder.Build(diagnostics);
        var sanitizedMethodName = key;

        foreach (var parameter in parameters)
        {
            var sanitizedParameterName = useParametersInMethodNames ? $"_{parameter.Name}_" : " ";
            sanitizedMethodName = sanitizedMethodName.Replace(parameter.Token, sanitizedParameterName);
        }

        var name = new MemberName(sanitizedMethodName.Trim());

        if (!name.IsValidMethodName())
        {
            var invalidName = name.ToString();
            name.MakeCompilable();

            diagnostics.Add(x => x.InvalidMemberName_0005(invalidName, name));
        }

        return new ExtensionMethodModel(rootNameSpace, markerType, key, value, name, parameters);
    }
}
