using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public struct MemorySlice<T>
    {
        private T[] _arr;
        private int _startPos;
        private int _length;
        public int Length => _length;

        public MemorySlice(T[] arr)
        {
            this._arr = arr;
            this._startPos = 0;
            this._length = arr.Length;
        }

        public Span<T> Span => _arr.AsSpan(start: _startPos, length: _length);

        public MemorySlice<T> Slice(int start)
        {
            return new MemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = _length - start
            };
        }

        public MemorySlice<T> Slice(int start, int length)
        {
            return new MemorySlice<T>()
            {
                _arr = _arr,
                _startPos = _startPos + start,
                _length = length
            };
        }
    }
}
