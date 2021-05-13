using System;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class FilePath_Tests
    {
        [Fact]
        public static void FilePathTypical()
        {
            new FilePath("Directory/Test.txt");
        }

        [Fact]
        public static void FilePathNoExtension()
        {
            new FilePath("Directory/Test");
        }

        [Fact]
        public static void FilePathJustFile()
        {
            new FilePath("Test.txt");
        }

        [Fact]
        public static void FilePathEmpty()
        {
            new FilePath(string.Empty);
        }
    }
}
