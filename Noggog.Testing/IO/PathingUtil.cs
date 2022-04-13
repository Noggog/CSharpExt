using System.Runtime.InteropServices;

namespace Noggog.Testing.IO
{
    public class PathingUtil
    {
        public static string DrivePrefix
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "C:\\";
                }
                else
                {
                    return "/";
                }
            }
        }
    }
}