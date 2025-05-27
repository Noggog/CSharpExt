using System.Runtime.CompilerServices;

namespace CSharpExt.UnitTests;

public static class TestHelper
{
    private static bool AutoVerify = false;

    private static VerifySettings GetVerifySettings()
    {
        var verifySettings = new VerifySettings();
#if DEBUG
        if (AutoVerify)
        {
            verifySettings.AutoVerify(includeBuildServer: true, throwException: false);
        }
#else
        verifySettings.DisableDiff();
#endif
        return verifySettings;
    }

    public static Task VerifyString(string str, [CallerFilePath] string sourceFile = "")
    {
        return Verifier.Verify(str, GetVerifySettings(), sourceFile);
    }
}