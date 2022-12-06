using System.Drawing;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace Noggog.WPF;

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
            filePath.Directory?.Create();
            save(settingsPath, mainVM);
            mainVM.Dispose();
        };
        window.DataContext = mainVM;
        return mainVM;
    }

    public static T WireMainVM<T>(
        this Window window,
        string settingsPath,
        JsonSerializerSettings? settings = null)
        where T : IDisposable, new()
    {
        return window.WireMainVM(
            settingsPath: settingsPath,
            load: (s) => (T)JsonConvert.DeserializeObject<T>(File.ReadAllText(s), settings)!,
            save: (s, vm) => File.WriteAllText(s, JsonConvert.SerializeObject(vm, Formatting.Indented, settings)));
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
            filePath.Directory?.Create();
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
            filePath.Directory?.Create();
            save(settingsPath, mainVM);
            mainVM.Dispose();
        };
        window.DataContext = mainVM;
        return mainVM;
    }

    public static void CenterAround(this Window window, Rectangle rectangle)
    {
        if (window.Width > rectangle.Width)
        {
            var diff = window.Width - rectangle.Width;
            diff /= 2;
            window.Left = rectangle.Left - diff;
        }
        else
        {
            var diff = rectangle.Width - window.Width;
            diff /= 2;
            window.Left = rectangle.Left + diff;
        }

        if (window.Top > rectangle.Top)
        {
            var diff = window.Height - rectangle.Height;
            diff /= 2;
            window.Top = rectangle.Top - diff;
        }
        else
        {
            var diff = rectangle.Height - window.Height;
            diff /= 2;
            window.Top = rectangle.Top + diff;
        }
    }
}