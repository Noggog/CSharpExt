using System.Collections;
using System.Linq;
using System.Windows.Controls;

namespace Noggog.WPF
{
    public static class ListBoxExt
    {
        public static bool TryRemoveSelected(this ListBox listBox)
        {
            if (listBox?.ItemsSource is not IList list) return false;
            foreach (var indexToRemove in listBox.GetChildrenOfType<ListBoxItem>()
                .WithIndex()
                .Where(x => x.Item.IsSelected)
                .Select(x => x.Index)
                .OrderByDescending(x => x)
                .ToArray())
            {
                list.RemoveAt(indexToRemove);
            }

            return true;
        }
        
        public static int IndexOf(this ListBox listBox, ListBoxItem listBoxItem)
        {
            var depth = listBox.GetChildDepth<ListBoxItem>();
            if (depth == -1) return -1;

            foreach (var potential in listBox.GetChildrenOfType<ListBoxItem>()
                .WithIndex())
            {
                if (ReferenceEquals(potential.Item, listBoxItem))
                {
                    return potential.Index;
                }
            }

            return -1;
        }
    }
}