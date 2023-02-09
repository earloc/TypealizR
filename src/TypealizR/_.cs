using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TypealizR.Tests")]

namespace TypealizR;

#pragma warning disable CA1707 // Identifiers should not contain underscores
public abstract class _
#pragma warning restore CA1707 // Identifiers should not contain underscores
{

    public const string RawSuffix = "_Raw";
    public const string KeySuffix = "_Key";
    public const string FallBackKeySuffix = "_FallbackKey";
}
