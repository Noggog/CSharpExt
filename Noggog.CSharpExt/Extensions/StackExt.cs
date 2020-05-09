using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Noggog
{
    public static class StackExt
    {
        public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T item)
        {
            if (stack.Count > 0)
            {
                item = stack.Pop();
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }

        public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T item)
        {
            if (stack.Count > 0)
            {
                item = stack.Peek();
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }
    }
}
