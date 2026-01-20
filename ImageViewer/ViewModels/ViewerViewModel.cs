using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Imaging;
using System;
using System.IO;

namespace ImageViewer.ViewModels;

public partial class ViewerViewModel : ObservableObject
{
    [ObservableProperty]
    private string? imagePath;

    [ObservableProperty]
    private BitmapImage? currentImage;

    [ObservableProperty]
    private double scale = 1.0;

    public event Action? EditRequested;

    // Metadata
    [ObservableProperty] private string resolution = "";
    [ObservableProperty] private string fileSize = "";
    [ObservableProperty] private string fileFormat = "";
    [ObservableProperty] private string creationDate = "";
    [ObservableProperty] private bool isMetadataVisible;

    // Slideshow
    private System.Windows.Threading.DispatcherTimer? _slideTimer;
    [ObservableProperty] private bool isSlideshowPlaying;

    // Color Picker
    [ObservableProperty] private bool isColorPickerActive;
    [ObservableProperty] private string pickedColorHex = "";

    public ViewerViewModel(string path)
    {
        ImagePath = path;
        LoadImage(path);
        LoadMetadata(path);
    }

    private void LoadMetadata(string path)
    {
        if (!File.Exists(path)) return;
        try 
        {
            var info = new FileInfo(path);
            FileSize = $"{info.Length / 1024.0:F2} KB";
            CreationDate = info.CreationTime.ToShortDateString();
            FileFormat = info.Extension.ToUpper();

            if (CurrentImage != null)
            {
                Resolution = $"{CurrentImage.PixelWidth} x {CurrentImage.PixelHeight}";
            }
        }
        catch {}
    }

    private void LoadImage(string path)
    {
        if (File.Exists(path))
        {
            try 
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                CurrentImage = bitmap;
            }
            catch (Exception)
            {
                // Handle bad image
            }
        }
    }

    [RelayCommand]
    private void ToggleMetadata() => IsMetadataVisible = !IsMetadataVisible;

    [RelayCommand]
    private void ToggleSlideshow()
    {
        IsSlideshowPlaying = !IsSlideshowPlaying;
        if (IsSlideshowPlaying)
        {
            _slideTimer = new System.Windows.Threading.DispatcherTimer();
            _slideTimer.Interval = TimeSpan.FromSeconds(3);
            _slideTimer.Tick += (s, e) => 
            {
                 // Trigger Next Image via MainViewModel (Loose Coupling needed or Event)
                 // Ideally send a request to MainVM to switch image.
                 // For now, let's fire an event
                 NextImageRequested?.Invoke();
            };
            _slideTimer.Start();
        }
        else
        {
            _slideTimer?.Stop();
        }
    }
    public event Action? NextImageRequested;

    [RelayCommand]
    private void ToggleColorPicker() => IsColorPickerActive = !IsColorPickerActive;

    [RelayCommand]
    private void OpenInExplorer()
    {
        if (File.Exists(ImagePath))
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{ImagePath}\"");
        }
    }

    [RelayCommand]
    private void CopyPath()
    {
        if (ImagePath != null) System.Windows.Clipboard.SetText(ImagePath);
    }
    
    [RelayCommand]
    private void ZoomIn() => Scale = Math.Min(Scale + 0.1, 5.0);

    [RelayCommand]
    private void ZoomOut() => Scale = Math.Max(Scale - 0.1, 0.1);

    [RelayCommand]
    private void Edit()
    {
        _slideTimer?.Stop(); // Stop slideshow on edit
        IsSlideshowPlaying = false;
        EditRequested?.Invoke();
    }
}
