using BenchmarkDotNet.Running;

namespace CSharpExt.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<BinaryMemoryReadStream>();
        BenchmarkRunner.Run<BinaryTests>();
    }
}