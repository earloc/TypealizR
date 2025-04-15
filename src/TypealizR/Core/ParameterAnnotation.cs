using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    internal static readonly IReadOnlyDictionary<string, string> NetStandard20SupportedTypes = new HashSet<string>([
         STRING,
         INT,
         UINT,
         LONG,
         ULONG,
         SHORT,
         USHORT,
         FLOAT,
         DOUBLE,
         DECIMAL,
         BOOL,
         DATETIME,
         DATETIMEOFFSET,
         GUID,
         URI,
        ]).ToFrozenDictionary(_ => _, _ => _);

    internal static readonly IReadOnlyDictionary<string, string> Net60SupportedTypes = new HashSet<string>([
         ..NetStandard20SupportedTypes.Keys,
         DATEONLY,
         TIMEONLY
    ]).ToFrozenDictionary(_ => _, _ => _);

    public static readonly IReadOnlyDictionary<string, string> Mappings = new Dictionary<string, string>() {
        { STRING, Net60SupportedTypes[STRING] },
        { STRING.ToUpperInvariant(), Net60SupportedTypes[STRING] },
        { typeof(string).Name, Net60SupportedTypes[STRING] },

        { "s", Net60SupportedTypes[STRING] },
        { "S", Net60SupportedTypes[STRING] },

        { INT, Net60SupportedTypes[INT] },
        { INT.ToUpperInvariant(), Net60SupportedTypes[INT] },
        { typeof(int).Name, Net60SupportedTypes[INT] },

        { "i", Net60SupportedTypes[INT] },
        { "I", Net60SupportedTypes[INT] },

        { UINT, Net60SupportedTypes[UINT] },
        { UINT.ToUpperInvariant(), Net60SupportedTypes[UINT] },
        { typeof(uint).Name, Net60SupportedTypes[UINT] },

        { "ui", Net60SupportedTypes[UINT] },
        { "UI", Net60SupportedTypes[UINT] },

        { LONG, Net60SupportedTypes[LONG] },
        { LONG.ToUpperInvariant(), Net60SupportedTypes[LONG] },
        { typeof(long).Name, Net60SupportedTypes[LONG] },

        { "l", Net60SupportedTypes[LONG] },
        { "L", Net60SupportedTypes[LONG] },


        { ULONG, Net60SupportedTypes[ULONG] },
        { ULONG.ToUpperInvariant(), Net60SupportedTypes[ULONG] },
        { typeof(ulong).Name, Net60SupportedTypes[ULONG] },

        { "ul", Net60SupportedTypes[ULONG] },
        { "UL", Net60SupportedTypes[ULONG] },

        { SHORT, Net60SupportedTypes[SHORT] },
        { SHORT.ToUpperInvariant(), Net60SupportedTypes[SHORT] },
        {typeof(short).Name, Net60SupportedTypes[SHORT] },

        { "sh", Net60SupportedTypes[SHORT] },
        { "SH", Net60SupportedTypes[SHORT] },

        { USHORT, Net60SupportedTypes[USHORT] },
        { USHORT.ToUpperInvariant(), Net60SupportedTypes[USHORT] },
        { typeof(ushort).Name, Net60SupportedTypes[USHORT] },

        { "ush", Net60SupportedTypes[USHORT] },
        { "USH", Net60SupportedTypes[USHORT] },

        { FLOAT, Net60SupportedTypes[FLOAT] },
        { FLOAT.ToUpperInvariant(), Net60SupportedTypes[FLOAT] },
        { typeof(float).Name, Net60SupportedTypes[FLOAT] },

        { "f", Net60SupportedTypes[FLOAT] },
        { "F", Net60SupportedTypes[FLOAT] },

        { DOUBLE, Net60SupportedTypes[DOUBLE] },
        { DOUBLE.ToUpperInvariant(), Net60SupportedTypes[DOUBLE] },
        { typeof(double).Name, Net60SupportedTypes[DOUBLE] },

        { "do", Net60SupportedTypes[DOUBLE] },
        { "DO", Net60SupportedTypes[DOUBLE] },

        { DECIMAL, Net60SupportedTypes[DECIMAL] },
        { DECIMAL.ToUpperInvariant(), Net60SupportedTypes[DECIMAL] },
        { typeof(decimal).Name, Net60SupportedTypes[DECIMAL] },

        { "dec", Net60SupportedTypes[DECIMAL] },
        { "DEC", Net60SupportedTypes[DECIMAL] },

        { BOOL, Net60SupportedTypes[BOOL] },
        { BOOL.ToUpperInvariant(), Net60SupportedTypes[BOOL] },
        { typeof(bool).Name, Net60SupportedTypes[BOOL] },

        { "b", Net60SupportedTypes[BOOL] },
        { "B", Net60SupportedTypes[BOOL] },

        { DATETIME, Net60SupportedTypes[DATETIME]  },
        { DATETIME.ToUpperInvariant(), Net60SupportedTypes[DATETIME]  },

        { "dt", Net60SupportedTypes[DATETIME] },
        { "DT", Net60SupportedTypes[DATETIME] },

        { DATETIMEOFFSET, Net60SupportedTypes[DATETIMEOFFSET] },
        { DATETIMEOFFSET.ToUpperInvariant(), Net60SupportedTypes[DATETIMEOFFSET] },

        { "dto", Net60SupportedTypes[DATETIMEOFFSET] },
        { "DTO", Net60SupportedTypes[DATETIMEOFFSET] },

        { GUID, Net60SupportedTypes[GUID] },
        { GUID.ToUpperInvariant(), Net60SupportedTypes[GUID] },

        { "g", Net60SupportedTypes[GUID] },
        { "G", Net60SupportedTypes[GUID] },

        { URI, Net60SupportedTypes[URI]  },
        { URI.ToUpperInvariant(), Net60SupportedTypes[URI]  },
        
        { DATEONLY, Net60SupportedTypes[DATEONLY]  },
        { DATEONLY.ToUpperInvariant(), Net60SupportedTypes[DATEONLY]  },

        { "d", Net60SupportedTypes[DATEONLY] },
        { "D", Net60SupportedTypes[DATEONLY] },

        { TIMEONLY, Net60SupportedTypes[TIMEONLY] },
        { TIMEONLY.ToUpperInvariant(), Net60SupportedTypes[TIMEONLY] },

        { "t", Net60SupportedTypes[TIMEONLY]},
        { "T", Net60SupportedTypes[TIMEONLY]},

        { "to", Net60SupportedTypes[TIMEONLY]},
        { "TO", Net60SupportedTypes[TIMEONLY]},

    }.ToFrozenDictionary();
}
