using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

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
            string settingsPath,
            JsonSerializerOptions? settings = null)
            where T : IDisposable, new()
        {
            return window.WireMainVM(
                settingsPath: settingsPath,
                load: (s) => (T)JsonSerializer.Deserialize<T>(File.ReadAllText(s), settings)!,
                save: (s, vm) => File.WriteAllText(s, JsonSerializer.Serialize(vm, settings)));
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
