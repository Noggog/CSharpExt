using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.UnitTests
{
    public static class Utility
    {
        public static readonly string TempFolderPath = "CSharpExtTests";
        
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
