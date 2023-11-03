using DynamicData;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq.Expressions;

namespace Noggog.WPF;

public static partial class Drag
{
    private static readonly object _relatedObj = new();
    public const string IsRelatedKey = "NoggogRelatedDrag";
    public const string SourceVmKey = "SourceVM";
    public const string SourceListControlKey = "SourceListControl";
    public const string SourceListIndexKey = "SourceListIndex";

    public static IObservable<(ListBoxItem?, Point?)> ConstructStartPoint(this Control control)
    {
        return Observable.Merge(
                control.Events().PreviewMouseLeftButtonDown
                    .Select(e =>
                    {
                        var item = VisualTreeHelper.HitTest(control, Mouse.GetPosition(control))?.VisualHit;
                        if (item == null || !item.TryGetAncestor<ListBoxItem>(out var hoveredItem))
                        {
                            return (default(ListBoxItem?), default(Point?));
                        }
                        return (hoveredItem, e.GetPosition(control));
                    }),
                control.Events().PreviewMouseLeftButtonUp
                    .Select(e => (default(ListBoxItem?), default(Point?))))
            .DistinctUntilChanged();
    }

    public static IObservable<DragEventArgs> ConstructBeginDrag(
        ListBox control,
        Action<ListBoxItem, Point> dragBegin)
    {
        var startPt = ConstructStartPoint(control)
            .Replay(1)
            .RefCount();

        return control.Events().MouseMove
            .FlowSwitch(startPt.Select(p => p.Item1 != null && p.Item2 != null))
            .Where(x => x.LeftButton == MouseButtonState.Pressed)
            .WithLatestFrom(
                startPt,
                (move, start) => (move, start))
            .Select(e =>
            {
                if (e.start.Item1 == null || e.start.Item2 == null) return default(DragEventArgs?);
                var startPt = e.start.Item2.Value;
                var position = e.move.GetPosition(control);
                if (Math.Abs(position.X - startPt.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(position.Y - startPt.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    dragBegin(e.start.Item1, startPt);
                }
                return default(DragEventArgs?);
            })
            .Merge(control.Events().Drop)
            .NotNull();
    }

    public static IObservable<DragEventParams<TType>> ListBoxDrops<TType>(
        ListBox control,
        Action<ListBoxItem, Point> dragBegin)
    {
        return ConstructParamExtraction<TType>(ConstructBeginDrag(control, dragBegin))
            .Replay(1)
            .RefCount();
    }

    public static IObservable<DragEventParams<TType>> ListBoxDrops<TType>(
        ListBox control,
        bool onlyWithinSameBox = true,
        Func<object, DragEventArgs, bool>? filter = null)
    {
        return Observable.Create<DragEventParams<TType>>((obs) =>
            {
                CompositeDisposable disp = new CompositeDisposable();
                control.Events().DragEnter
                    .Merge(control.Events().DragOver)
                    .Subscribe(e => CanDropCheck(e.Source, e, onlyWithinSameBox, filter))
                    .DisposeWith(disp);
                ConstructParamExtraction<TType>(ConstructBeginDrag(control, (item, startPt) => TypicalBeginDrag(control, item, startPt)))
                    .Subscribe(o => obs.OnNext(o))
                    .DisposeWith(disp);
                return disp;
            })
            .Replay(1)
            .RefCount();
    }

    private static IObservable<DragEventParams<TType>> ConstructParamExtraction<TType>(IObservable<DragEventArgs> args)
    {
        return args
            .Select(e =>
            {
                var ret = new DragEventParams<TType>(e);
                if (e.Data.GetDataPresent(SourceVmKey))
                {
                    TType? vmTarget;
                    var rawData = e.Data.GetData(SourceVmKey);
                    // Keep as is, to support non-objects
                    if (rawData is TType ttype)
                    {
                        vmTarget = ttype;
                    }
                    else if (rawData is ISelectedItem<TType> selectedItem)
                    {
                        vmTarget = selectedItem.Item;
                    }
                    else
                    {
                        vmTarget = default;
                    }
                    ret = ret with
                    {
                        Vm = vmTarget
                    };
                }
                if (e.Data.GetDataPresent(SourceListControlKey))
                {
                    ret = ret with
                    {
                        SourceListBox = e.Data.GetData(SourceListControlKey) as ListBox
                    };
                }
                if (e.Data.GetDataPresent(SourceListIndexKey))
                {
                    ret = ret with
                    {
                        SourceListIndex = (int)e.Data.GetData(SourceListIndexKey)
                    };
                }
                else
                {
                    ret = ret with
                    {
                        SourceListIndex = -1
                    };
                }
                return ret;
            });
    }

    private static void HandleDragDropEvent<TType>(DragEventParams<TType> e)
        where TType : notnull
    {
        if (e.Vm == null) return;
        if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
        if (!dep.TryGetAncestor<ListBoxItem>(out var listBoxItem)) return;
            
        if (!listBoxItem.TryGetAncestor<ListBox>(out var listBox)) return;

        if (e.SourceListBox == null) return;

        if (!TryGetAsDragDropSource<TType>(e.SourceListBox.ItemsSource, out var originatingSource)) return;
        if (!TryGetAsDragDropTarget<TType>(listBox.ItemsSource, out var targetSource)) return;

        if (e.SourceListIndex != -1)
        {
            originatingSource.RemoveAt(e.SourceListIndex);
        }
        else
        {
            originatingSource.Remove(e.Vm);
        }

        var hoverPt = e.RawArgs.GetPosition(listBoxItem);
        var before = hoverPt.Y < listBoxItem.ActualHeight / 2;
            
        targetSource.InsertAtTarget(listBoxItem.DataContext, e.Vm, before);
    }

    public static IDisposable ListBoxDragDrop<TViewModel, TType>(
        ListBox control,
        TViewModel vm,
        Expression<Func<TViewModel, IList<TType>?>> vmProperty,
        bool onlyWithinSameBox = true,
        Func<object, DragEventArgs, bool>? filter = null)
        where TType : notnull
    {
        return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
            .Subscribe(HandleDragDropEvent);
    }

    public static IDisposable ListBoxDragDrop<TViewModel, TType>(
        ListBox control,
        TViewModel vm,
        Expression<Func<TViewModel, ISourceList<TType>?>> vmProperty,
        bool onlyWithinSameBox = true,
        Func<object, DragEventArgs, bool>? filter = null)
        where TType : notnull
    {
        return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
            .Subscribe(HandleDragDropEvent);
    }

    public static IDisposable ListBoxDragDrop<TType>(
        ListBox control,
        bool onlyWithinSameBox = true,
        Func<object, DragEventArgs, bool>? filter = null)
        where TType : notnull
    {
        return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
            .Subscribe(HandleDragDropEvent);
    }

    public static IDisposable ListBoxDragDrop<TViewModel, TType>(
        ListBox control,
        TViewModel vm,
        Expression<Func<TViewModel, IList<TType>?>> vmProperty,
        Action<ListBoxItem, Point> dragBegin)
        where TType : notnull
    {
        return ListBoxDrops<TType>(control, dragBegin)
            .Subscribe(HandleDragDropEvent);
    }

    public static IDisposable ListBoxDragDrop<TViewModel, TType>(
        ListBox control,
        TViewModel vm,
        Expression<Func<TViewModel, ISourceList<TType>?>> vmProperty,
        Action<ListBoxItem, Point> dragBegin)
        where TType : notnull
    {
        return ListBoxDrops<TType>(control, dragBegin)
            .Subscribe(HandleDragDropEvent);
    }

    public static IDisposable ListBoxDragDrop<TType>(
        ListBox control,
        Action<ListBoxItem, Point> dragBegin)
        where TType : notnull
    {
        return ListBoxDrops<TType>(control, dragBegin)
            .Subscribe(HandleDragDropEvent);
    }

    private static void CanDropCheck(
        object sender,
        DragEventArgs e,
        bool onlyWithinSameBox,
        Func<object, DragEventArgs, bool>? filter = null)
    {
        if ((filter != null && !filter(sender, e))
            || !e.Data.GetDataPresent(IsRelatedKey)
            || !e.Data.GetDataPresent(SourceVmKey))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return;
        }
        var sourceListBox = e.Data.GetData(SourceListControlKey) as ListBox;
        if (onlyWithinSameBox
            && (sender is not Control senderControl
                || !senderControl.TryGetAncestor<ListBox>(out var senderListBox)
                || !ReferenceEquals(sourceListBox, senderListBox)))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return;
        }
    }

    public static void TypicalBeginDrag(
        ListBox listBox,
        ListBoxItem listBoxItem,
        Point startPoint)
    {
        //setup the drag adorner.
        var adorner = InitialiseAdorner(listBoxItem, listBox);

        //add handles to update the adorner.
        DragEventHandler previewDrag = (object sender, DragEventArgs e) =>
        {
            adorner.OffsetLeft = e.GetPosition(listBox).X;
            adorner.OffsetTop = e.GetPosition(listBox).Y - startPoint.Y;
        };
        listBox.PreviewDragOver += previewDrag;

        DataObject data = new DataObject(SourceVmKey, listBoxItem.DataContext);
        data.SetData(IsRelatedKey, _relatedObj);
        data.SetData(SourceListControlKey, listBox);
            
        if (listBoxItem.DataContext is ISelected sel
            && listBox.ItemsSource is IReadOnlyList<ISelected> selectableList)
        {
            data.SetData(SourceListIndexKey, selectableList.IndexOf(sel));
        }
            
        DragDropEffects de = DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
            
        //cleanup
        listBox.PreviewDragOver -= previewDrag;

        AdornerLayer.GetAdornerLayer(listBox)?.Remove(adorner);
    }

    public static DragAdorner InitialiseAdorner(ListBoxItem listBoxItem, Control control)
    {
        VisualBrush brush = new VisualBrush(listBoxItem);
        var adorner = new DragAdorner((UIElement)listBoxItem, listBoxItem.RenderSize, brush);
        adorner.Opacity = 0.5;
        var layer = AdornerLayer.GetAdornerLayer(control);
        layer.Add(adorner);
        return adorner;
    }
}