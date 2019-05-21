using System;
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

        public static unsafe float GetFloat(this ReadOnlySpan<byte> span)
        {
            // ToDo
            // Swap for BinaryPrimitives when implemented
            // https://github.com/dotnet/corefx/issues/35791
            fixed (byte* ptr = &MemoryMarshal.GetReference(span))
            {
                return *(float*)ptr;
            }
        }

        public static unsafe double GetDouble(this ReadOnlySpan<byte> span)
        {
            // ToDo
            // Swap for BinaryPrimitives when implemented
            // https://github.com/dotnet/corefx/issues/35791
            fixed (byte* ptr = &MemoryMarshal.GetReference(span))
            {
                return *(double*)ptr;
            }
        }
    }
}
