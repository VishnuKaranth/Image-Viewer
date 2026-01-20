using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageViewer.Converters;

public class PathToBitmapConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrEmpty(path) && File.Exists(path))
        {
            try 
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                bool isStandard = ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif" || ext == ".ico" || ext == ".tif" || ext == ".tiff";

                if (isStandard)
                {
                    // Standard WPF Load
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Releases file lock
                    bitmap.UriSource = new Uri(path);
                    bitmap.DecodePixelWidth = 200; // Thumbnail optimization
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                else
                {
                    // Magick.NET Load for advanced formats
                    using var magickImage = new ImageMagick.MagickImage(path);
                    
                    // Optimization: Create a thumbnail version for the gallery
                    // Calculate thumbnail size (maintain aspect ratio, fit within 200x200)
                    if (magickImage.Width > 200 || magickImage.Height > 200)
                    {
                        magickImage.Resize(200, 0); 
                    }

                    // Convert to BitmapSource using MemoryStream (Universal method)
                    byte[] bytes = magickImage.ToByteArray(ImageMagick.MagickFormat.Bmp); // Convert to BMP byte array
                    var ms = new MemoryStream(bytes);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (Exception)
            {
                // Fallback or log?
                return null;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
