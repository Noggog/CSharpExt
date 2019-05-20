using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace CSharpExt.Benchmark
{
    public class BinaryTests
    {
        static byte[] arr = new byte[] { 0, 1, 2, 3, 4 };
        static Memory<byte> mem = arr.AsMemory();

        [Benchmark]
        public int GetIntBitConverter()
        {
            return BitConverter.ToInt32(arr, 1);
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesMemorySlice()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(mem.Slice(1).Span);
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesMemorySliceToSize()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(mem.Slice(1, 4).Span);
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesMemorySpanSlice()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(mem.Span.Slice(1));
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesMemorySpanSliceToSize()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(mem.Span.Slice(1, 4));
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesSlice()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(arr.AsSpan().Slice(1));
        }

        [Benchmark]
        public int GetIntBinaryPrimitivesSliceToSize()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(arr.AsSpan().Slice(1, 4));
        }
    }
}
