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
        public static T WireMainVM<T>(this Window window)
            where T : new()
        {
            T mainVM = new T();
            window.DataContext = mainVM;
            return mainVM;
        }

        public static T WireMainVM<T>(
            this Window window,
            string settingsPath,
            Func<string, T> load,
            Action<string, T> save)
            where T : IDisposable, new()
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
                mainVM.Dispose();
            };
            window.DataContext = mainVM;
            return mainVM;
        }

        public static T WireMainVM<T>(
            this Window window,
            string settingsPath)
            where T : IDisposable, new()
        {
            return window.WireMainVM(
                settingsPath: settingsPath,
                load: (s) => JsonConvert.DeserializeObject<T>(File.ReadAllText(s)),
                save: (s, vm) => File.WriteAllText(s, JsonConvert.SerializeObject(vm, Formatting.Indented)));
        }

        public static T WireMainVM<T>(
            this Window window,
            string settingsPath,
            T mainVM,
            Action<string, T> load,
            Action<string, T> save)
            where T : IDisposable
        {
            if (File.Exists(settingsPath))
            {
                load(settingsPath, mainVM);
            }
            window.Closed += (a, b) =>
            {
                FilePath filePath = new FilePath(settingsPath);
                filePath.Directory.Create();
                save(settingsPath, mainVM);
                mainVM.Dispose();
            };
            window.DataContext = mainVM;
            return mainVM;
        }

        public static T WireMainVM<T>(
            this Window window,
            string settingsPath,
            Action<string, T> load,
            Action<string, T> save)
            where T : IDisposable, new()
        {
            var mainVM = new T();
            if (File.Exists(settingsPath))
            {
                load(settingsPath, mainVM);
            }
            window.Closed += (a, b) =>
            {
                FilePath filePath = new FilePath(settingsPath);
                filePath.Directory.Create();
                save(settingsPath, mainVM);
                mainVM.Dispose();
            };
            window.DataContext = mainVM;
            return mainVM;
        }
    }
}
