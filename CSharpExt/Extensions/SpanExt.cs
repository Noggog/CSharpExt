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

        /// <summary>
        /// Retrieves an Int8 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Int8 from the given span from the desired index</returns>
        public static sbyte GetInt8(this ReadOnlySpan<byte> span, int index = 0)
        {
            return (sbyte)span[index];
        }

        /// <summary>
        /// Retrieves an Int16 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Int16 from the given span from the desired index</returns>
        public static short GetInt16(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, short>(span.Slice(index, 2))[0];
        }

        /// <summary>
        /// Retrieves an Int32 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Int32 from the given span from the desired index</returns>
        public static int GetInt32(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, int>(span.Slice(index, 4))[0];
        }

        public static int GetInt32(this ReadOnlySpan<byte> span)
        {
            return MemoryMarshal.Cast<byte, int>(span)[0];
        }

        /// <summary>
        /// Retrieves an Int64 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Int64 from the given span from the desired index</returns>
        public static long GetInt64(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, long>(span.Slice(index, 8))[0];
        }

        /// <summary>
        /// Retrieves an UInt16 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>UInt16 from the given span from the desired index</returns>
        public static ushort GetUInt16(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, ushort>(span.Slice(index, 2))[0];
        }

        /// <summary>
        /// Retrieves an UInt32 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>UInt32 from the given span from the desired index</returns>
        public static uint GetUInt32(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, uint>(span.Slice(index, 4))[0];
        }

        /// <summary>
        /// Retrieves an UInt64 from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>UInt64 from the given span from the desired index</returns>
        public static ulong GetUInt64(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, ulong>(span.Slice(index, 8))[0];
        }

        /// <summary>
        /// Retrieves a Float from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Float from the given span from the desired index</returns>
        public static float GetFloat(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, float>(span.Slice(index, 4))[0];
        }

        /// <summary>
        /// Retrieves a Double from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Double from the given span from the desired index</returns>
        public static double GetDouble(this ReadOnlySpan<byte> span, int index = 0)
        {
            return MemoryMarshal.Cast<byte, double>(span.Slice(index, 8))[0];
        }

        /// <summary>
        /// Retrieves a Boolean from the given span starting at desired index.
        /// NOTE:  Index is relative to the byte span input
        /// 
        /// Boolean is any byte value greater than zero
        /// </summary>
        /// <param name="span">Span to retrieve value from</param>
        /// <param name="index">Index relative to input span to retrieve from</param>
        /// <returns>Boolean from the given span from the desired index</returns>
        public static bool GetBool(this ReadOnlySpan<byte> span, int index = 0)
        {
            return span[index] > 0;
        }
    }
}
