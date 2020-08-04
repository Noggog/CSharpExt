using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Noggog.WPF
{
    public static class ImageUtility
    {
        public static BitmapImage BitmapImageFromResource(string assemblyName, string resourceName) => BitmapImageFromStream(System.Windows.Application.GetResourceStream(new Uri($"pack://application:,,,/{assemblyName};component/{resourceName}")).Stream);

        public static BitmapImage BitmapImageFromStream(Stream stream)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = stream;
            img.EndInit();
            img.Freeze();
            return img;
        }

        public static bool TryGetBitmapImageFromFile(string path, [MaybeNullWhen(false)] out BitmapImage bitmapImage)
        {
            try
            {
                if (!File.Exists(path))
                {
                    bitmapImage = default;
                    return false;
                }
                bitmapImage = new BitmapImage(new Uri((string)path, UriKind.RelativeOrAbsolute));
                return true;
            }
            catch (Exception)
            {
                bitmapImage = default;
                return false;
            }
        }
    }
}
