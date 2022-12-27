using System.Runtime.CompilerServices;

namespace TypealizR.Tests.Snapshots;

internal interface IVerifiable
{
    Task Verify([CallerMemberName] string caller = "");
}
