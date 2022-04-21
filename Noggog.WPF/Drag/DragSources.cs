using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Noggog.WPF.Containers;
using Noggog.WPF.Internals;

namespace Noggog.WPF
{
    public static partial class Drag
    {
        static bool TryGetAsDragDropSource<T>(
            object obj,
            [MaybeNullWhen(false)] out IDragDropSource<T> source)
        {
            if (obj is ISourceList<T> sourceList)
            {
                source = new SourceListDragDropSource<T>(sourceList);
                return true;
            }
            if (obj is ISourceListUiFunnel<T> funnel)
            {
                source = new SourceListDragDropSource<T>(funnel.SourceList);
                return true;
            }
            if (obj is IDerivativeSelectedCollection<T> derivative)
            {
                if (derivative.OriginalList == null)
                {
                    source = default;
                    return false;
                }
                else
                {
                    source = new ListDragDropSource<T>(derivative.OriginalList);
                    return true;
                }
            }
            if (obj is IList<T> l)
            {
                source = new ListDragDropSource<T>(l);
                return true;
            }

            source = default;
            return false;
        }
    }

    namespace Internals
    {
        interface IDragDropSource<T>
        {
            void RemoveAt(int index);
            void Remove(T item);
        }

        class ListDragDropSource<T> : IDragDropSource<T>
        {
            private readonly IList<T> _list;

            public ListDragDropSource(IList<T> list)
            {
                _list = list;
            }

            public void Remove(T item)
            {
                _list.Remove(item);
            }

            public void RemoveAt(int index)
            {
                _list.RemoveAt(index);
            }
        }

        class SourceListDragDropSource<T> : IDragDropSource<T>
        {
            private readonly ISourceList<T> _sourceList;

            public SourceListDragDropSource(ISourceList<T> sourceList)
            {
                _sourceList = sourceList;
            }

            public void Remove(T item)
            {
                _sourceList.Remove(item);
            }

            public void RemoveAt(int index)
            {
                _sourceList.RemoveAt(index);
            }
        }
    }
}