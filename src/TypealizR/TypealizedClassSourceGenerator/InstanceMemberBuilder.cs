using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis;
using System.Xml.Linq;
using TypealizR.Extensions;
using TypealizR.Diagnostics;
using TypealizR.Core;namespace TypealizR;
internal class InstanceMemberBuilder
{    private readonly bool useParametersInMethodNames;    private readonly string key;
    private readonly string rawKey;
    private readonly string value;
    private readonly ParameterBuilder parameterBuilder;

    public InstanceMemberBuilder(bool useParametersInMethodNames, string key, string rawKey, string value)
    {        this.useParametersInMethodNames = useParametersInMethodNames;        this.key = key;
        this.rawKey = rawKey;
        this.value = value;
        parameterBuilder = new(key);
    }

    public InstanceMemberModel Build(DiagnosticsCollector diagnostics)
    {
        var parameters = parameterBuilder.Build(diagnostics);

        var sanitizedMethodName = key;

        foreach (var parameter in parameters)
        {            var sanitizedParameterName = useParametersInMethodNames ? $"_{parameter.Name}_" : " ";
            sanitizedMethodName = sanitizedMethodName.Replace(parameter.Token, sanitizedParameterName);
        }

        var name = new MemberName(sanitizedMethodName.Trim());        if (!name.IsValidMethodName())        {            var invalidName = name.ToString();            name.MakeCompilable();            diagnostics.Add(x => x.InvalidMemberName_0005(name, invalidName));        }

        return new InstanceMemberModel(rawKey, value, name, parameters);    }

}
