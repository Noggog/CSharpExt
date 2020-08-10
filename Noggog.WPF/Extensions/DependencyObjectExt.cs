using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Noggog.WPF
{
    public static class DependencyObjectExt
    {
        public static bool TryGetAncestor<TObj>(this DependencyObject obj, [MaybeNullWhen(false)] out TObj foundObj, bool testSelf = true)
            where TObj : DependencyObject
        {
            DependencyObject? item = obj;
            while (item != null && !(item is TObj))
                item = VisualTreeHelper.GetParent(item);
            foundObj = item as TObj;
            return foundObj != null;
        }

        public static TObj? GetAncestor<TObj>(this DependencyObject obj, bool testSelf = true)
            where TObj : DependencyObject
        {
            if (TryGetAncestor<TObj>(obj, out var ancestor))
            {
                return ancestor;
            }
            return default;
        }
    }
}
