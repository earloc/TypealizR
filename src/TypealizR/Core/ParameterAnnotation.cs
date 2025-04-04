using System.Collections.Frozen;
using System.Collections.Generic;

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
        { STRING.ToUpperInvariant(), SupportedTypes[STRING] },

        { "s", SupportedTypes[STRING] },
        { "S", SupportedTypes[STRING] },

        { INT, SupportedTypes[INT] },
        { INT.ToUpperInvariant(), SupportedTypes[INT] },

        { "i", SupportedTypes[INT] },
        { "I", SupportedTypes[INT] },

        { UINT, SupportedTypes[UINT] },
        { UINT.ToUpperInvariant(), SupportedTypes[UINT] },

        { "ui", SupportedTypes[UINT] },
        { "UI", SupportedTypes[UINT] },

        { LONG, SupportedTypes[LONG] },
        { LONG.ToUpperInvariant(), SupportedTypes[LONG] },

        { "l", SupportedTypes[LONG] },
        { "L", SupportedTypes[LONG] },


        { ULONG, SupportedTypes[ULONG] },
        { ULONG.ToUpperInvariant(), SupportedTypes[ULONG] },

        { "ul", SupportedTypes[ULONG] },
        { "UL", SupportedTypes[ULONG] },

        { SHORT, SupportedTypes[SHORT] },
        { SHORT.ToUpperInvariant(), SupportedTypes[SHORT] },

        { "sh", SupportedTypes[SHORT] },
        { "SH", SupportedTypes[SHORT] },

        { USHORT, SupportedTypes[USHORT] },
        { USHORT.ToUpperInvariant(), SupportedTypes[USHORT] },

        { "ush", SupportedTypes[USHORT] },
        { "USH", SupportedTypes[USHORT] },


        { SINGLE, SupportedTypes[SINGLE] },
        { SINGLE.ToUpperInvariant(), SupportedTypes[SINGLE] },

        { "si", SupportedTypes[SINGLE] },
        { "SI", SupportedTypes[SINGLE] },

        { FLOAT, SupportedTypes[FLOAT] },
        { FLOAT.ToUpperInvariant(), SupportedTypes[FLOAT] },

        { "f", SupportedTypes[FLOAT] },
        { "F", SupportedTypes[FLOAT] },

        { DOUBLE, SupportedTypes[DOUBLE] },
        { DOUBLE.ToUpperInvariant(), SupportedTypes[DOUBLE] },

        { "do", SupportedTypes[DOUBLE] },
        { "DO", SupportedTypes[DOUBLE] },

        { DECIMAL, SupportedTypes[DECIMAL] },
        { DECIMAL.ToUpperInvariant(), SupportedTypes[DECIMAL] },

        { "dec", SupportedTypes[DECIMAL] },
        { "DEC", SupportedTypes[DECIMAL] },

        { BOOL, SupportedTypes[BOOL] },
        { BOOL.ToUpperInvariant(), SupportedTypes[BOOL] },

        { "b", SupportedTypes[BOOL] },
        { "B", SupportedTypes[BOOL] },

        { DATETIME, SupportedTypes[DATETIME]  },
        { DATETIME.ToUpperInvariant(), SupportedTypes[DATETIME]  },

        { "dt", SupportedTypes[DATETIME] },
        { "DT", SupportedTypes[DATETIME] },

        { DATETIMEOFFSET, SupportedTypes[DATETIMEOFFSET] },
        { DATETIMEOFFSET.ToUpperInvariant(), SupportedTypes[DATETIMEOFFSET] },

        { "dto", SupportedTypes[DATETIMEOFFSET] },
        { "DTO", SupportedTypes[DATETIMEOFFSET] },

        { GUID, SupportedTypes[GUID] },
        { GUID.ToUpperInvariant(), SupportedTypes[GUID] },

        { "g", SupportedTypes[GUID] },
        { "G", SupportedTypes[GUID] },
        
        { URI, SupportedTypes[URI]  },
        { URI.ToUpperInvariant(), SupportedTypes[URI]  },
        
        //TODO: how opt-ing out of > .net6 types
        { DATEONLY, SupportedTypes[DATEONLY]  },
        { DATEONLY.ToUpperInvariant(), SupportedTypes[DATEONLY]  },

        { "d", SupportedTypes[DATEONLY] },
        { "D", SupportedTypes[DATEONLY] },

        { TIMEONLY, SupportedTypes[TIMEONLY] },
        { TIMEONLY.ToUpperInvariant(), SupportedTypes[TIMEONLY] },

        { "t", SupportedTypes[TIMEONLY]},
        { "T", SupportedTypes[TIMEONLY]},

        // TODO: possible breaking change, when removing previous
        { "to", SupportedTypes[TIMEONLY]},
        { "TO", SupportedTypes[TIMEONLY]},

    }.ToFrozenDictionary();
}
