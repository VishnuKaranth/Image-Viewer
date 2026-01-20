using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input; // For Rect

namespace ImageViewer.ViewModels;

public partial class EditorViewModel : ObservableObject
{
    [ObservableProperty]
    private BitmapSource? displayImage;

    [ObservableProperty]
    private double rotationAngle = 0;

    // Filters
    [ObservableProperty]
    private double brightness = 0; // -100 to 100

    [ObservableProperty]
    private double contrast = 1.0; // 0.2 to 5.0 (1.0 is normal)

    private readonly string _originalPath;
    private BitmapSource? _baseImage; // Image after load/rotation, before filters

    public EditorViewModel(string imagePath)
    {
        _originalPath = imagePath;
        LoadImage(imagePath);
    }

    private void LoadImage(string path)
    {
         if (File.Exists(path))
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            
            _baseImage = bitmap;
            UpdateDisplay();
        }
    }

    partial void OnBrightnessChanged(double value) => UpdateDisplay();
    partial void OnContrastChanged(double value) => UpdateDisplay();

    [RelayCommand]
    private void RotateLeft() => ApplyRotation(-90);

    [RelayCommand]
    private void RotateRight() => ApplyRotation(90);

    [RelayCommand]
    private void FlipHorizontal() => ApplyScale(-1, 1);

    [RelayCommand]
    private void FlipVertical() => ApplyScale(1, -1);

    [RelayCommand]
    private void Crop(Rect rect)
    {
        if (DisplayImage == null || rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) return;

        // Rect is in UI coord space (displayed image size). Need to map to actual pixel size.
        // NOTE: In the View, we should ideally pass Normalized Coordinates (0..1) to handle scaling safely.
        
        // Assuming we receive Normalized Rect (0.0 to 1.0)
        int x = (int)(rect.X * _baseImage!.PixelWidth);
        int y = (int)(rect.Y * _baseImage!.PixelHeight);
        int w = (int)(rect.Width * _baseImage!.PixelWidth);
        int h = (int)(rect.Height * _baseImage!.PixelHeight);

        // Safety clamps
        x = Math.Max(0, x);
        y = Math.Max(0, y);
        w = Math.Min(w, _baseImage.PixelWidth - x);
        h = Math.Min(h, _baseImage.PixelHeight - y);

        if (w > 0 && h > 0)
        {
            var cropped = new CroppedBitmap(_baseImage, new Int32Rect(x, y, w, h));
            cropped.Freeze();
            _baseImage = cropped; // Commit crop
            UpdateDisplay();
        }
    }

    private void ApplyRotation(double angle)
    {
        if (_baseImage != null)
        {
            var transformed = new TransformedBitmap(_baseImage, new RotateTransform(angle));
            transformed.Freeze();
            _baseImage = transformed; // Commit rotation as new base
            UpdateDisplay();
        }
    }

    private void ApplyScale(double scaleX, double scaleY)
    {
        if (_baseImage != null)
        {
            var transformed = new TransformedBitmap(_baseImage, new ScaleTransform(scaleX, scaleY));
            transformed.Freeze();
            _baseImage = transformed;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (_baseImage == null) return;

        // Verify if filters are active
        if (Math.Abs(Brightness) < 0.1 && Math.Abs(Contrast - 1.0) < 0.01)
        {
            DisplayImage = _baseImage;
            return;
        }

        // Apply Brightness/Contrast efficiently
        DisplayImage = ApplyFilters(_baseImage, Brightness, Contrast);
    }

    private BitmapSource ApplyFilters(BitmapSource source, double brightness, double contrast)
    {
        // Ensure format is BGRA32 for consistent pixel manipulation
        if (source.Format != PixelFormats.Bgra32)
        {
            source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
        }

        int width = source.PixelWidth;
        int height = source.PixelHeight;
        int stride = width * 4; 
        byte[] pixels = new byte[height * stride];

        source.CopyPixels(pixels, stride, 0);

        // Precompute LUTs (Look Up Tables) for speed
        byte[] lut = new byte[256];
        double b = brightness; 
        double c = contrast; 

        for (int i = 0; i < 256; i++)
        {
            double val = i;
            // Contrast
            val = (val - 128) * c + 128;
            // Brightness
            val += b;

            lut[i] = (byte)Math.Clamp(val, 0, 255);
        }

        // Apply LUT to RGB channels (skip Alpha)
        for (int i = 0; i < pixels.Length; i += 4)
        {
            pixels[i] = lut[pixels[i]];     // B
            pixels[i + 1] = lut[pixels[i + 1]]; // G
            pixels[i + 2] = lut[pixels[i + 2]]; // R
            // pixels[i+3] is Alpha, ignore
        }

        var result = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
        result.Freeze();
        return result;
    }

    [RelayCommand]
    private void SaveImage()
    {
        if (DisplayImage == null) return;

        var encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(DisplayImage));

        try 
        {
            // Force release of any file handles held by previous bitmaps
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using var stream = new FileStream(_originalPath, FileMode.Create);
            encoder.Save(stream);
            System.Windows.MessageBox.Show("Image Saved!", "Success");
            
            // Re-load to confirm saved state becomes new base
            LoadImage(_originalPath);
            Brightness = 0;
            Contrast = 1.0;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving image: {ex.Message}", "Error");
        }
    }
}
