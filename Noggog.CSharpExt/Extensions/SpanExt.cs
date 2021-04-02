using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Noggog
{
    public static class SpanExt
    {
        public static ReadOnlySpan<sbyte> AsSByteSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, sbyte>(span);
        }

        public static ReadOnlySpan<short> AsInt16Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, short>(span);
        }

        public static ReadOnlySpan<int> AsInt32Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, int>(span);
        }

        public static ReadOnlySpan<long> AsInt64Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, long>(span);
        }

        public static ReadOnlySpan<ushort> AsUInt16Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, ushort>(span);
        }

        public static ReadOnlySpan<uint> AsUInt32Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, uint>(span);
        }

        public static ReadOnlySpan<ulong> AsUInt64Span(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, ulong>(span);
        }

        public static ReadOnlySpan<float> AsFloatSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, float>(span);
        }

        public static ReadOnlySpan<double> AsDoubleSpan(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, double>(span);
        }

        public static float Float(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadSingleLittleEndian(span);
        }

        public static unsafe double Double(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadDoubleLittleEndian(span);
        }

        public static unsafe string StringUTF8(this ReadOnlySpan<byte> span)
        {
            fixed (byte* buffer = &MemoryMarshal.GetReference(span))
            {
                return Encoding.UTF8.GetString(buffer, span.Length);
            }
        }

        public static byte UInt8(this ReadOnlySpan<byte> span)
        {
            return span[0];
        }

        public static sbyte Int8(this ReadOnlySpan<byte> span)
        {
            return (sbyte)span[0];
        }

        public static ushort UInt16(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static short Int16(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static uint UInt32(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static int Int32(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static ulong UInt64(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public static long Int64(this ReadOnlySpan<byte> span)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public static float Float(this ReadOnlyMemorySlice<byte> span) => BinaryPrimitives.ReadSingleLittleEndian(span);

        public static double Double(this ReadOnlyMemorySlice<byte> span) => BinaryPrimitives.ReadDoubleLittleEndian(span);

        public static string StringUTF8(this ReadOnlyMemorySlice<byte> span) => span.Span.StringUTF8();

        public static byte UInt8(this ReadOnlyMemorySlice<byte> span)
        {
            return span[0];
        }

        public static sbyte Int8(this ReadOnlyMemorySlice<byte> span)
        {
            return (sbyte)span[0];
        }

        public static ushort UInt16(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static short Int16(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public static uint UInt32(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static int Int32(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public static ulong UInt64(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public static long Int64(this ReadOnlyMemorySlice<byte> span)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public static unsafe string ToHexString(this ReadOnlySpan<byte> span)
        {
            var lookupP = ByteExt.Lookup32UnsafeP;
            var result = new char[span.Length * 2];
            fixed (char* resultP = result)
            {
                uint* resultP2 = (uint*)resultP;
                for (int i = 0; i < span.Length; i++)
                {
                    resultP2[i] = lookupP[span[i]];
                }
            }
            return new string(result);
        }

        public static unsafe string ToHexString(this ReadOnlyMemorySlice<byte> span)
        {
            return ToHexString(span.Span);
        }

        public static unsafe string ToHexString(this MemorySlice<byte> span)
        {
            return ToHexString(span.Span);
        }

        public static unsafe string ToHexString(this Span<byte> span)
        {
            return ToHexString((ReadOnlySpan<byte>)span);
        }
    }
}
