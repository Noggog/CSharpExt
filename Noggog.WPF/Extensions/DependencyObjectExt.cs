using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Noggog.WPF;

public static class DependencyObjectExt
{
    public static bool TryGetAncestor<TObj>(this DependencyObject obj, [MaybeNullWhen(false)] out TObj foundObj, bool testSelf = true)
        where TObj : DependencyObject
    {
        DependencyObject? item = obj;
        while (item is not null && item is not TObj)
        {
            try
            {
                item = VisualTreeHelper.GetParent(item);
            }
            catch (InvalidOperationException e)
            {
                break;
            }
        }
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

    public static bool TryGetChildOfType<TObj>(this DependencyObject depObj, [MaybeNullWhen(false)] out TObj foundObj)
        where TObj : DependencyObject
    {
        foundObj = GetChildOfType<TObj>(depObj);
        return foundObj != null;
    }

    public static TObj? GetChildOfType<TObj>(this DependencyObject depObj)
        where TObj : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = (child as TObj) ?? GetChildOfType<TObj>(child);
            if (result != null) return result;
        }
        return null;
    }
        
    public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject depObj, int? maxDepth = null)
        where T : DependencyObject
    {
        if (maxDepth == 0) yield break;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
        {
            DependencyObject? child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t)
                yield return t;
            foreach (T visualChild in child.GetChildrenOfType<T>(maxDepth == null ? null : maxDepth.Value - 1))
                yield return visualChild;
        }
    }
        
    public static int GetChildDepth<T>(this DependencyObject depObj)
        where T : DependencyObject
    {
        return GetChildDepth<T>(depObj, startDepth: 1);
    }
        
    private static int GetChildDepth<T>(DependencyObject depObj, int startDepth)
        where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
        {
            DependencyObject? child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t)
                return startDepth;
            var childDepth = GetChildDepth<T>(child, startDepth + 1);
            if (childDepth != -1)
            {
                return childDepth;
            }
        }

        return -1;
    }
}