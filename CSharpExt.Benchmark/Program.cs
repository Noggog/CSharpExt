using BenchmarkDotNet.Running;
using System;

namespace CSharpExt.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BinaryMemoryReadStream>();
        }
    }
}
