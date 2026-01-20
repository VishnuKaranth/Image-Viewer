using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using ImageViewer.Services;
using ImageViewer.Views;
using System.Threading.Tasks;

namespace ImageViewer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentView;

    [ObservableProperty]
    private string windowTitle = "Photo Viewer";

    private readonly IImageService _imageService;
    public GalleryViewModel GalleryVM { get; }

    public MainViewModel()
    {
        _imageService = new ImageService();
        GalleryVM = new GalleryViewModel(_imageService);
        GalleryVM.OpenImageRequested += OpenViewer;
        GalleryVM.PickFolderRequested += async () => await OpenFolder();
        GalleryVM.PickImageRequested += async () => await OpenSingleImage();
        
        // Sets the View to GalleryView, binding Context to GalleryVM
        CurrentView = new GalleryView { DataContext = GalleryVM };  
    }



    private void OpenViewer(string imagePath)
    {
        var viewerVM = new ViewerViewModel(imagePath);
        viewerVM.EditRequested += () => OpenEditor(imagePath);
        viewerVM.NextImageRequested += NextImage; // Link Slideshow to NextImage logic
        CurrentView = new ViewerView { DataContext = viewerVM };
    }

    public async Task OpenFile(string path)
    {
        if (System.IO.Directory.Exists(path))
        {
            WindowTitle = $"Photo Viewer - {path}";
            await GalleryVM.LoadImagesAsync(path);
            CurrentView = new GalleryView { DataContext = GalleryVM };
        }
        else if (System.IO.File.Exists(path))
        {
            // If just a file, load it. But also try to load siblings for navigation?
            // For now, just open the file. 
            // Ideally we find the parent folder, load it in background, and select this image.
            
            var parent = System.IO.Path.GetDirectoryName(path);
            if (parent != null)
            {
                 // Load context silently
                 await GalleryVM.LoadImagesAsync(parent);
                 GalleryVM.SelectedImage = path;
            }
            OpenViewer(path);
        }
    }

    [RelayCommand]
    private void NextImage()
    {
        if (CurrentView is ViewerView && GalleryVM.Images.Any())
        {
            var currentIndex = GalleryVM.Images.IndexOf(GalleryVM.SelectedImage ?? "");
            if (currentIndex != -1 && currentIndex < GalleryVM.Images.Count - 1)
            {
                var nextPath = GalleryVM.Images[currentIndex + 1];
                GalleryVM.SelectedImage = nextPath; // Update selection
                OpenViewer(nextPath); // Re-open viewer with new image
            }
        }
    }

    [RelayCommand]
    private void PreviousImage()
    {
        if (CurrentView is ViewerView && GalleryVM.Images.Any())
        {
            var currentIndex = GalleryVM.Images.IndexOf(GalleryVM.SelectedImage ?? "");
            if (currentIndex > 0)
            {
                var prevPath = GalleryVM.Images[currentIndex - 1];
                GalleryVM.SelectedImage = prevPath;
                OpenViewer(prevPath);
            }
        }
    }

    private void OpenEditor(string imagePath)
    {
        var editorVM = new EditorViewModel(imagePath);
        // editorVM.SaveComplete += ... (Optional: Go back to viewer on save)
        CurrentView = new EditorView { DataContext = editorVM };
    }

    [RelayCommand]
    private void GoBack()
    {
        GalleryVM.SelectedImage = null; // Deselect to allow re-selection
        CurrentView = new GalleryView { DataContext = GalleryVM };
    }

    [RelayCommand]
    private async Task OpenSingleImage()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff|All Files|*.*";
        if (dialog.ShowDialog() == true)
        {
            await OpenFile(dialog.FileName);
        }
    }

    [RelayCommand]
    private async Task OpenFolder()
    {   
        using var dialog = new FolderBrowserDialog();
        DialogResult result = dialog.ShowDialog();

        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
        {
            WindowTitle = $"Photo Viewer - {dialog.SelectedPath}";
            await GalleryVM.LoadImagesAsync(dialog.SelectedPath);
        }
    }
}
