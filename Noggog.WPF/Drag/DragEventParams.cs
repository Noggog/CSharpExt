using System.Windows;
using System.Windows.Controls;

namespace Noggog.WPF
{
    public static partial class Drag
    {
        public record DragEventParams<TViewModel>(DragEventArgs RawArgs)
        {
            public TViewModel? Vm { get; set; }
            public ListBox? SourceListBox { get; set; }
            public int SourceListIndex { get; set; }
        }
    }
}