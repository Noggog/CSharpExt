using System;
using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class DirectoryPath_Tests
    {
        [Fact]
        public static void Typical()
        {
            new DirectoryPath("Directory/Test.txt");
        }

        [Fact]
        public static void NoExtension()
        {
            new DirectoryPath("Directory/Test");
        }

        [Fact]
        public static void JustDirectory()
        {
            new DirectoryPath("Test.txt");
        }

        [Fact]
        public static void Empty()
        {
            new DirectoryPath(string.Empty);
        }

        [Fact]
        public static void Equal()
        {
            new DirectoryPath("Directory/Test")
                .Should().BeEquivalentTo(
                    new DirectoryPath("Directory/Test/"));
        }
    }
}
