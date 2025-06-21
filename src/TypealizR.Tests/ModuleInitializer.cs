using System.Runtime.CompilerServices;

namespace TypealizR.Tests;

#pragma warning disable CA1515 // Consider making public types internal
public static class ModuleInitializer
#pragma warning restore CA1515 // Consider making public types internal
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffEngine.DiffTools.UseOrder(DiffEngine.DiffTool.VisualStudioCode);

        VerifySourceGenerators.Initialize();
    }
}