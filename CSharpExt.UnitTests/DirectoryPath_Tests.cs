using System;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class DirectoryPath_Tests
    {
        [Fact]
        public static void DirectoryPathTypical()
        {
            new DirectoryPath("Directory/Test.txt");
        }

        [Fact]
        public static void DirectoryPathNoExtension()
        {
            new DirectoryPath("Directory/Test");
        }

        [Fact]
        public static void DirectoryPathJustDirectory()
        {
            new DirectoryPath("Test.txt");
        }

        [Fact]
        public static void DirectoryPathEmpty()
        {
            new DirectoryPath(string.Empty);
        }
    }
}
