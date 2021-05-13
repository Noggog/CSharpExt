using System;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class FileName_Tests
    {
        [Fact]
        public static void FileNameTypical()
        {
            new FileName("Test.txt");
        }

        [Fact]
        public static void FileNameNoExtension()
        {
            new FileName("Test");
        }

        [Fact]
        public static void FileNameHasDirectory()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new FileName("Directory/Test.txt");
            });
        }

        [Fact]
        public static void FileNameEmpty()
        {
            new FileName(string.Empty);
        }
    }
}
