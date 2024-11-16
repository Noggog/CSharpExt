using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using Autofac.Features.OwnedInstances;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Noggog;
using Noggog.SourceGenerators.AssemblyVersion;

namespace CSharpExt.UnitTests.SourceGenerators;

public class AssemblyVersionGeneratorTests
{
    public class SourceGenerationTestHelper
    {
        private static bool AutoVerify = false;

        private static VerifySettings GetVerifySettings()
        {
            var verifySettings = new VerifySettings();
    #if DEBUG
            if (AutoVerify)
            {
                verifySettings.AutoVerify();
            }
    #else
            verifySettings.DisableDiff();
    #endif
            return verifySettings;
        }
        
        public static Task VerifySerialization(string source, [CallerFilePath] string sourceFile = "")
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        
            IEnumerable<PortableExecutableReference> references = new[]
            {
                MetadataReference.CreateFromFile(typeof(Owned<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            };
            
            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // Create an instance of our incremental source generator
            var generator = new AssemblyVersionGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        
            // Run the source generator!
            driver = driver.RunGenerators(compilation);
            
            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver, GetVerifySettings(), sourceFile);
        }

        public static GeneratorDriverRunResult RunSourceGenerator(string source)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            IEnumerable<PortableExecutableReference> references = new[]
            {
                MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            };

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // Create an instance of our incremental source generator
            var generator = new AssemblyVersionGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);
            
            return driver.GetRunResult();
        }
    }

    [Fact]
    public async Task BasicGeneration()
    {
        var source = @"

namespace System.Runtime.CompilerServices
{
    public sealed class IsExternalInit
    {
    }
}";
        await SourceGenerationTestHelper.VerifySerialization(source);
    }

    [Fact]
    public async Task BasicUsage()
    {
        var source = @"
namespace SomeNamespace
{
    public class MyClass
    {
        public AssemblyVersions Test() => AssemblyVersions.For<MyClass>();
    }
}

namespace System.Runtime.CompilerServices
{
    public sealed class IsExternalInit
    {
    }
}";
        await SourceGenerationTestHelper.VerifySerialization(source);
    }

    [Fact]
    public async Task NamespaceSpecified()
    {
        var source = @"
namespace SomeNamespace
{
    public class MyClass
    {
        public AssemblyVersions Test() => AssemblyVersions.For<SomeNamespace.MyClass>();
    }
}

namespace System.Runtime.CompilerServices
{
    public sealed class IsExternalInit
    {
    }
}";

        await SourceGenerationTestHelper.VerifySerialization(source);
    }

    [Fact]
    public async Task GenericPassed()
    {
        var source = @"
namespace SomeNamespace
{
    public class MyClass
    {
        public AssemblyVersions Test<T>() => AssemblyVersions.For<T>();
    }
}

namespace System.Runtime.CompilerServices
{
    public sealed class IsExternalInit
    {
    }
}";

        await SourceGenerationTestHelper.VerifySerialization(source);
    }
}