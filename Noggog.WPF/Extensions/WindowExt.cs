using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Noggog.WPF
{
    public static class WindowExt
    {
        public static void WireMainVM<T>(
            this Window window,
            T mainVM,
            string settingsPath,
            Action<string, T> load,
            Action<string, T> save)
        {
            window.Closed += (a, b) =>
            {
                FilePath filePath = new FilePath(settingsPath);
                filePath.Directory.Create();
                save(settingsPath, mainVM);
            };
            window.DataContext = mainVM;
            load(settingsPath, mainVM);
        }
    }
}
