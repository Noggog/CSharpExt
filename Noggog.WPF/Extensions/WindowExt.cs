using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Noggog.WPF
{
    public static class WindowExt
    {
        public static T WireMainVM<T>(
            this Window window,
            string settingsPath,
            Func<string, T> load,
            Action<string, T> save)
            where T : new()
        {
            T mainVM;
            if (File.Exists(settingsPath))
            {
                mainVM = load(settingsPath);
            }
            else
            {
                mainVM = new T();
            }
            window.Closed += (a, b) =>
            {
                FilePath filePath = new FilePath(settingsPath);
                filePath.Directory.Create();
                save(settingsPath, mainVM);
            };
            window.DataContext = mainVM;
            return mainVM;
        }

        public static T WireMainVM<T>(
            this Window window,
            string settingsPath)
            where T : new()
        {
            return window.WireMainVM(
                settingsPath: settingsPath,
                load: (s) => JsonConvert.DeserializeObject<T>(File.ReadAllText(s)),
                save: (s, vm) => File.WriteAllText(s, JsonConvert.SerializeObject(vm, Formatting.Indented)));
        }
    }
}
