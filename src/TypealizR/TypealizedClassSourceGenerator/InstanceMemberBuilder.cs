using TypealizR.Core;
using TypealizR.Diagnostics;

namespace TypealizR;
internal class InstanceMemberBuilder
{
    private readonly string rootNameSpace;
    private readonly bool useParametersInMethodNames;
    private readonly string key;
    private readonly string rawKey;
    private readonly string value;
    private readonly ParameterBuilder parameterBuilder;

    public InstanceMemberBuilder(string rootNameSpace, bool useParametersInMethodNames, string key, string rawKey, string value)
    {
        this.rootNameSpace = rootNameSpace;
        this.useParametersInMethodNames = useParametersInMethodNames;
        this.key = key;
        this.rawKey = rawKey;
        this.value = value;
        parameterBuilder = new(key);
    }

    public InstanceMemberModel Build(DiagnosticsCollector diagnostics)
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

        return new InstanceMemberModel(rootNameSpace, rawKey, value, name, parameters);
    }

}
