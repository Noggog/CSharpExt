using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Noggog
{
    public struct MemorySlice<T>
    {
        private T[] _arr;
        private int _startPos;
        private int _length;
        public int Length => _length;
        public int StartPosition => _startPos;

        [DebuggerStepThrough]
        public MemorySlice(T[] arr)
        {
            this._arr = arr;
            this._startPos = 0;
            this._length = arr.Length;
        }

        [DebuggerStepThrough]
        public MemorySlice(T[] arr, int startPos, int length)
        {
            this._arr = arr;
            this._startPos = startPos;
            this._length = length;
        }

        public Span<T> Span => _arr.AsSpan(start: _startPos, length: _length);

        [DebuggerStepThrough]
        public MemorySlice<T> Slice(int start)
        {
            return new MemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = _length - start
            };
        }

        [DebuggerStepThrough]
        public MemorySlice<T> Slice(int start, int length)
        {
            return new MemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = length
            };
        }

        public static implicit operator ReadOnlyMemorySlice<T>(MemorySlice<T> mem)
        {
            return new ReadOnlyMemorySlice<T>(
                mem._arr,
                mem._startPos,
                mem._length);
        }

        public static implicit operator ReadOnlySpan<T>(MemorySlice<T> mem)
        {
            return mem.Span;
        }

        public static implicit operator Span<T>(MemorySlice<T> mem)
        {
            return mem.Span;
        }
    }

    public struct ReadOnlyMemorySlice<T>
    {
        private T[] _arr;
        private int _startPos;
        private int _length;
        public int Length => _length;
        public int StartPosition => _startPos;

        [DebuggerStepThrough]
        public ReadOnlyMemorySlice(T[] arr)
        {
            this._arr = arr;
            this._startPos = 0;
            this._length = arr.Length;
        }

        [DebuggerStepThrough]
        public ReadOnlyMemorySlice(T[] arr, int startPos, int length)
        {
            this._arr = arr;
            this._startPos = startPos;
            this._length = length;
        }

        public ReadOnlySpan<T> Span => _arr.AsSpan(start: _startPos, length: _length);

        [DebuggerStepThrough]
        public ReadOnlyMemorySlice<T> Slice(int start)
        {
            return new ReadOnlyMemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = _length - start
            };
        }

        [DebuggerStepThrough]
        public ReadOnlyMemorySlice<T> Slice(int start, int length)
        {
            return new ReadOnlyMemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = length
            };
        }

        public static implicit operator ReadOnlySpan<T>(ReadOnlyMemorySlice<T> mem)
        {
            return mem.Span;
        }
    }
}
