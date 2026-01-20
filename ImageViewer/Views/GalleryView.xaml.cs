using System.Windows.Controls;

namespace ImageViewer.Views;

public partial class GalleryView : System.Windows.Controls.UserControl
{
    public GalleryView()
    {
        InitializeComponent();
    }

    private void OpenFolder_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.GalleryViewModel vm)
        {
            vm.RequestOpenFolderCommand.Execute(null);
        }
    }

    private void OpenImage_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.GalleryViewModel vm)
        {
            vm.RequestOpenImageCommand.Execute(null);
        }
    }
}
