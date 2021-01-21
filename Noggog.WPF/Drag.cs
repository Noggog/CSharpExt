using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Noggog.WPF
{
    public static class Drag
    {
        private readonly static object _relatedObj = new object();
        public const string IsRelatedKey = "NoggogRelatedDrag";
        public const string SourceVmKey = "SourceVM";
        public const string SourceListControlKey = "SourceListControl";

        public record DragEventParams<TViewModel>(DragEventArgs RawArgs)
        {
            public TViewModel? Vm { get; set; }
            public ListBox? SourceListBox { get; set; }
        }

        public static IObservable<(ListBoxItem?, Point?)> ConstructStartPoint(this Control control)
        {
            return Observable.Merge(
                    control.Events().PreviewMouseLeftButtonDown
                        .Select(e =>
                        {
                            var item = VisualTreeHelper.HitTest(control, Mouse.GetPosition(control)).VisualHit;
                            if (!item.TryGetAncestor<ListBoxItem>(out var hoveredItem))
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
                .FilterSwitch(startPt.Select(p => p.Item1 != null && p.Item2 != null))
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
            where TType : class
        {
            return ConstructParamExtraction<TType>(ConstructBeginDrag(control, dragBegin))
                .Replay(1)
                .RefCount();
        }

        public static IObservable<DragEventParams<TType>> ListBoxDrops<TType>(
            ListBox control,
            bool onlyWithinSameBox = true,
            Func<object, DragEventArgs, bool>? filter = null)
            where TType : class
        {
            return Observable.Create<DragEventParams<TType>>((obs) =>
                {
                    CompositeDisposable disp = new CompositeDisposable();
                    control.Events().DragEnter
                        .Merge(control.Events().DragOver)
                        .Subscribe(e => CanDropCheck(e.Source, e, onlyWithinSameBox, filter))
                        .DisposeWith(disp);
                    ConstructParamExtraction<TType>(ConstructBeginDrag(control, (item, startPt) => TypicalBeginDrag(control, item, startPt, onlyWithinSameBox, filter)))
                        .Subscribe(o => obs.OnNext(o))
                        .DisposeWith(disp);
                    return disp;
                })
                .Replay(1)
                .RefCount();
        }

        private static IObservable<DragEventParams<TType>> ConstructParamExtraction<TType>(IObservable<DragEventArgs> args)
            where TType : class
        {
            return args
                .Select(e =>
                {
                    var ret = new DragEventParams<TType>(e);
                    if (e.Data.GetDataPresent(SourceVmKey))
                    {
                        ret = ret with
                        {
                            Vm = e.Data.GetData(SourceVmKey) as TType
                        };
                    }
                    if (e.Data.GetDataPresent(SourceListControlKey))
                    {
                        ret = ret with
                        {
                            SourceListBox = e.Data.GetData(SourceListControlKey) as ListBox
                        };
                    }
                    return ret;
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<IList<TType>?> vmListGetter,
            bool onlyWithinSameBox = true,
            Func<object, DragEventArgs, bool>? filter = null)
            where TType : class
        {
            return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<ISourceList<TType>?> vmListGetter,
            bool onlyWithinSameBox = true,
            Func<object, DragEventArgs, bool>? filter = null)
            where TType : class
        {
            return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.Items.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<ObservableCollection<TType>?> vmListGetter,
            bool onlyWithinSameBox = true,
            Func<object, DragEventArgs, bool>? filter = null)
            where TType : class
        {
            return ListBoxDrops<TType>(
                control,
                onlyWithinSameBox: onlyWithinSameBox,
                filter: filter)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<IList<TType>?> vmListGetter,
            Action<ListBoxItem, Point> dragBegin)
            where TType : class
        {
            return ListBoxDrops<TType>(control, dragBegin)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<ISourceList<TType>?> vmListGetter,
            Action<ListBoxItem, Point> dragBegin)
            where TType : class
        {
            return ListBoxDrops<TType>(control, dragBegin)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.Items.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
        }

        public static IDisposable ListBoxDragDrop<TType>(
            ListBox control,
            Func<ObservableCollection<TType>?> vmListGetter,
            Action<ListBoxItem, Point> dragBegin)
            where TType : class
        {
            return ListBoxDrops<TType>(control, dragBegin)
                .Subscribe(e =>
                {
                    if (e.Vm == null) return;
                    if (e.RawArgs.OriginalSource is not DependencyObject dep) return;
                    if (!dep.TryGetAncestor<ListBoxItem>(out var targetItem)) return;
                    if (targetItem.DataContext is not TType vm) return;
                    var list = vmListGetter();
                    if (list == null) return;
                    var index = list.IndexOf(vm);

                    if (index >= 0)
                    {
                        list.Remove(e.Vm);
                        list.Insert(index, e.Vm);
                    }
                    else
                    {
                        list.Remove(e.Vm);
                        list.Add(e.Vm);
                    }
                });
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
            Point startPoint,
            bool onlyWithinSameBox,
            Func<object, DragEventArgs, bool>? filter = null)
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
            DragDropEffects de = DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);

            //cleanup
            listBox.PreviewDragOver -= previewDrag;

            AdornerLayer.GetAdornerLayer(listBox).Remove(adorner);
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
}
