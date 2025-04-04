using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TypealizR;

public class ParameterAnnotation
{
    public ParameterAnnotation(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public string Type { get; }

    private const string STRING = "string";
    private const string INT = "int";
    private const string UINT = "uint";
    private const string LONG = "long";
    private const string ULONG = "ulong";
    private const string SHORT = "short";
    private const string USHORT = "ushort";
    private const string SINGLE = "single";
    private const string FLOAT = "float";
    private const string DOUBLE = "double";
    private const string DECIMAL = "decimal";
    private const string BOOL = "bool";
    private const string DATETIME = "DateTime";
    private const string DATEONLY = "DateOnly";
    private const string TIMEONLY = "TimeOnly";
    private const string DATETIMEOFFSET = "DateTimeOffset";
    private const string GUID = "Guid";
    private const string URI = "Uri";
    internal static readonly IReadOnlyDictionary<string, string> SupportedTypes = new HashSet<string>() {
         STRING,
         INT,
         UINT,
         LONG,
         ULONG,
         SHORT,
         USHORT,
         SINGLE,
         FLOAT,
         DOUBLE,
         DECIMAL,
         BOOL,
         DATETIME,
         DATETIMEOFFSET,
         GUID,
         URI,
         // TODO: how opt-ing out of > .net6 types
         DATEONLY,
         TIMEONLY
    }.ToFrozenDictionary(_ => _, _ => _);

    public static readonly IReadOnlyDictionary<string, string> Mappings = new Dictionary<string, string>() {
        { STRING, SupportedTypes[STRING] },
        { "s", SupportedTypes[STRING] },

        { INT, SupportedTypes[INT] },
        { "i", SupportedTypes[INT] },

        { UINT, SupportedTypes[UINT] },
        { "ui", SupportedTypes[UINT] },

        { LONG, SupportedTypes[LONG] },
        { "l", SupportedTypes[LONG] },

        { ULONG, SupportedTypes[ULONG] },
        { "ul", SupportedTypes[ULONG] },

        { SHORT, SupportedTypes[SHORT] },
        { "sh", SupportedTypes[SHORT] },

        { USHORT, SupportedTypes[USHORT] },
        { "ush", SupportedTypes[USHORT] },

        { SINGLE, SupportedTypes[SINGLE] },
        { "si", SupportedTypes[SINGLE] },

        { FLOAT, SupportedTypes[FLOAT] },
        { "f", SupportedTypes[FLOAT] },

        { DOUBLE, SupportedTypes[DOUBLE] },
        { "d", SupportedTypes[DOUBLE] },

        { DECIMAL, SupportedTypes[DECIMAL] },
        { "dec", SupportedTypes[DECIMAL] },

        { BOOL, SupportedTypes[BOOL] },
        { "b", SupportedTypes[BOOL] },

        { DATETIME, SupportedTypes[DATETIME]  },
        { "dt", SupportedTypes[DATETIME] },

        { DATETIMEOFFSET, SupportedTypes[DATETIMEOFFSET] },
        { "dto", SupportedTypes[DATETIMEOFFSET] },

        { GUID, SupportedTypes[GUID] },
        { "g", SupportedTypes[GUID] },
        
        { URI,SupportedTypes[URI]  },
        
        //TODO: how opt-ing out of > .net6 types
        { DATEONLY, SupportedTypes[DATEONLY]  },
        { "do", SupportedTypes[DATEONLY] },
        { TIMEONLY, SupportedTypes[TIMEONLY] },
        { "to", SupportedTypes[TIMEONLY]},
    }.ToFrozenDictionary();
}
