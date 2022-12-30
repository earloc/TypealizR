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
using TypealizR.Values;

namespace TypealizR;
internal class InstanceMemberBuilder
{
    private readonly string key;
    private readonly string rawKey;
    private readonly string value;
    private readonly ParameterBuilder parameterBuilder;

    public InstanceMemberBuilder(string key, string rawKey, string value)
    {
        this.key = key;
        this.rawKey = rawKey;
        this.value = value;
        parameterBuilder = new(key);
    }

    public InstanceMemberModel Build(DiagnosticsCollector diagnostics)
    {
        var parameters = parameterBuilder.Build(diagnostics);

        var methodNameWithoutParameters = key;

        foreach (var parameter in parameters)
        {
            methodNameWithoutParameters = methodNameWithoutParameters.Replace(parameter.Token, $"_{parameter.Name}_");
        }

        var name = new MemberName(methodNameWithoutParameters.Trim());

        return new InstanceMemberModel(rawKey, value, name, parameters);
    }

}
