using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TypealizR;

public class ParameterAnnotation
{
    public ParameterAnnotation(string name, Type type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public Type Type { get; }

    public static readonly ParameterAnnotation[] KnownAnnotations = [
        new ("string", typeof(string)),
        new ("int", typeof(int)),
        new ("long", typeof(long)),
        new ("float", typeof(float)),
        new ("double", typeof(double)),
        new ("decimal", typeof(decimal)),
        new ("bool", typeof(bool)),
        new ("DateTime", typeof(DateTime)),
        new ("DateTimeOffset", typeof(DateTimeOffset)),
        new ("Guid", typeof(Guid)),
        new ("Uri", typeof(Uri))
    ];

    public static readonly IEnumerable<Type> KonwTypes = KnownAnnotations
        .Select(x => x.Type)
        .Distinct()
        .ToArray()
    ;
}
