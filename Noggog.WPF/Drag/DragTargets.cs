using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Noggog.WPF.Containers;
using Noggog.WPF.Internals;

namespace Noggog.WPF
{
    public static partial class Drag
    {
        static bool TryGetAsDragDropTarget<T>(
            object obj,
            [MaybeNullWhen(false)] out IDragDropTarget<T> target)
        {
            if (obj is ISourceList<T> sourceList)
            {
                target = new SourceListDragDropTarget<T>(sourceList);
                return true;
            }
            if (obj is ISourceListUiFunnel<T> funnel)
            {
                target = new SourceListDragDropTarget<T>(funnel.SourceList);
                return true;
            }
            if (obj is IDerivativeSelectedCollection<T> derivative)
            {
                target = new DerivativeListDragDropTarget<T>(derivative);
                return true;
            }
            if (obj is IList<T> l)
            {
                target = new ListDragDropTarget<T>(l);
                return true;
            }
            target = default;
            return false;
        }
    }

    namespace Internals
    {
        interface IDragDropTarget<T>
        {
            void InsertAtTarget(object target, T item, bool before);
        }

        class ListDragDropTarget<T> : IDragDropTarget<T>
        {
            private readonly IList<T> _list;

            public ListDragDropTarget(IList<T> list)
            {
                _list = list;
            }

            public void InsertAtTarget(object target, T item, bool before)
            {
                if (target is not T targetItem) return;
                var targetIndex = _list.IndexOf(targetItem);
                if (targetIndex >= _list.Count) return;

                if (targetIndex >= 0)
                {
                    if (!before)
                    {
                        targetIndex++;
                    }
                    _list.Insert(targetIndex, item);
                }
                else
                {
                    _list.Add(item);
                }
            }
        }

        class SourceListDragDropTarget<T> : IDragDropTarget<T>
        {
            private readonly ISourceList<T> _sourceList;

            public SourceListDragDropTarget(ISourceList<T> sourceList)
            {
                _sourceList = sourceList;
            }

            public void InsertAtTarget(object target, T item, bool before)
            {
                if (target is not T targetItem) return;
                var targetIndex = _sourceList.Items.IndexOf(targetItem);
                if (targetIndex >= _sourceList.Count) return;

                if (targetIndex >= 0)
                {
                    if (!before)
                    {
                        targetIndex++;
                    }
                    _sourceList.Insert(targetIndex, item);
                }
                else
                {
                    _sourceList.Add(item);
                }
            }
        }

        class DerivativeListDragDropTarget<T> : IDragDropTarget<T>
        {
            private readonly IDerivativeSelectedCollection<T> _list;

            public DerivativeListDragDropTarget(IDerivativeSelectedCollection<T> list)
            {
                _list = list;
            }

            public void InsertAtTarget(object target, T item, bool before)
            {
                if (target is not ISelectedItem<T> selTarget) return;
                if (_list.OriginalList == null) return;
                
                var targetIndex = _list.DerivativeList.IndexOf(selTarget);
                if (targetIndex >= _list.OriginalList.Count) return;

                if (targetIndex >= 0)
                {
                    if (!before)
                    {
                        targetIndex++;
                    }
                    _list.OriginalList.Insert(targetIndex, item);
                }
                else
                {
                    _list.OriginalList.Add(item);
                }
            }
        }
    }
}