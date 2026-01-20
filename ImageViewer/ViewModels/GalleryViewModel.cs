using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ImageViewer.Services;
using System.Threading.Tasks;

namespace ImageViewer.ViewModels;

public partial class GalleryViewModel : ObservableObject
{
    private readonly IImageService _imageService;
    
    [ObservableProperty]
    private ObservableCollection<string> images = new();

    [ObservableProperty]
    private string? selectedImage;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = "Open a folder to view images";

    public event Action<string>? OpenImageRequested;
    public event Action? PickFolderRequested;
    public event Action? PickImageRequested;

    public GalleryViewModel(IImageService imageService)
    {
        _imageService = imageService;
    }

    [RelayCommand]
    private void OpenImage(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            OpenImageRequested?.Invoke(path);
        }
    }

    [RelayCommand]
    private void RequestOpenFolder()
    {
        PickFolderRequested?.Invoke();
    }

    [RelayCommand]
    private void RequestOpenImage()
    {
        PickImageRequested?.Invoke();
    }

    public async Task LoadImagesAsync(string folderPath)
    {
        IsLoading = true;
        Images.Clear();

        await Task.Run(() =>
        {
            try
            {
                var files = _imageService.GetImagesInFolder(folderPath).ToList(); // Materialize list to count
                // System.Windows.MessageBox.Show($"Found {files.Count} files in {folderPath}");
                
                foreach (var file in files)
                {
                    // Dispatch to UI thread
                     System.Windows.Application.Current.Dispatcher.Invoke(() => Images.Add(file));
                }
            }
            catch (Exception ex)
            {
                 System.Windows.Application.Current.Dispatcher.Invoke(() => StatusMessage = $"Error: {ex.Message}");
            }
        });

        if (Images.Count == 0)
        {
            StatusMessage = "No images found in this folder.";
        }
        else
        {
            StatusMessage = $"{Images.Count} images loaded.";
        }

        IsLoading = false;
    }
}
