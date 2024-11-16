using System.Runtime.CompilerServices;

namespace CSharpExt.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        VerifyDiffPlex.Initialize();
    }
}