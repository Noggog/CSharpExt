using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace CSharpExt.Benchmark
{
    [MemoryDiagnoser]
    public class BinaryTests
    {
        static byte[] arr = new byte[] { 0, 1, 2, 3, 4 };
        static Memory<byte> mem = arr.AsMemory();
        static byte[] largeArray;

        [GlobalSetup]
        public void Setup()
        {
            largeArray = new byte[4096];
            for (int i = 0; i < largeArray.Length; i++)
            {
                largeArray[i] = (byte)(i % 256);
            }
        }

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

        [Benchmark]
        public byte[] GetBytes()
        {
            byte[] ret = new byte[arr.Length];
            Array.Copy(arr, 0, ret, 0, arr.Length);
            return ret;
        }

        [Benchmark]
        public byte[] GetBytesViaSpan()
        {
            return arr.AsSpan().ToArray();
        }

        [Benchmark]
        public byte[] GetSomeBytes()
        {
            byte[] ret = new byte[arr.Length - 1];
            Array.Copy(arr, 2, ret, 0, arr.Length - 2);
            return ret;
        }

        [Benchmark]
        public byte[] GetSomeBytesViaSpan()
        {
            return arr.AsSpan().Slice(1).ToArray();
        }

        [Benchmark]
        public byte[] GetLargeBytes()
        {
            byte[] ret = new byte[largeArray.Length];
            Array.Copy(largeArray, 0, ret, 0, largeArray.Length);
            return ret;
        }

        [Benchmark]
        public byte[] GetLargeBytesViaSpan()
        {
            return largeArray.AsSpan().ToArray();
        }

        [Benchmark]
        public byte[] GetSomeLargeBytes()
        {
            byte[] ret = new byte[largeArray.Length - 10];
            Array.Copy(largeArray, 5, ret, 0, largeArray.Length - 10);
            return ret;
        }

        [Benchmark]
        public byte[] GetSomeLargeBytesViaSpan()
        {
            return largeArray.AsSpan(5, largeArray.Length - 10).ToArray();
        }
    }
}
