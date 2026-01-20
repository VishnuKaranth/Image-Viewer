using System.Windows.Controls;
using System.Windows.Media.Imaging; // Added for CroppedBitmap, FormatConvertedBitmap

namespace ImageViewer.Views;

public partial class ViewerView : System.Windows.Controls.UserControl
{
    public ViewerView()
    {
        InitializeComponent();
    }

    private void UserControl_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (DataContext is ViewModels.ViewerViewModel vm)
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (e.Delta > 0) vm.ZoomInCommand.Execute(null);
                else vm.ZoomOutCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (DataContext is ViewModels.ViewerViewModel vm && vm.IsColorPickerActive)
        {
            if (sender is System.Windows.Controls.Image img && img.Source is BitmapSource bitmap)
            {
                // Set cursor
                img.Cursor = System.Windows.Input.Cursors.Pen;

                // Get mouse position relative to image control
                var pos = e.GetPosition(img);

                // Calculate actual pixel coordinates
                // Image is stretched "Uniform", so we need to map UI coords to Pixel coords
                // But since we are getting position relative to 'img' which IS the image element,
                // and the image element size might differ from actual bitmap size due to scaling/stretching.
                
                // Actually, 'img' properties ActualWidth/Height are the displayed size.
                // 'bitmap' properties PixelWidth/Height.
                
                double xRatio = bitmap.PixelWidth / img.ActualWidth;
                double yRatio = bitmap.PixelHeight / img.ActualHeight;

                int x = (int)(pos.X * xRatio);
                int y = (int)(pos.Y * yRatio);

                if (x >= 0 && x < bitmap.PixelWidth && y >= 0 && y < bitmap.PixelHeight)
                {
                    // Copy 1x1 pixel
                    byte[] pixels = new byte[4];
                    try 
                    {
                        var cb = new CroppedBitmap(bitmap, new System.Windows.Int32Rect(x, y, 1, 1));
                        // Ensure BGRA
                        if (cb.Format != System.Windows.Media.PixelFormats.Bgra32)
                        {
                             var converted = new FormatConvertedBitmap(cb, System.Windows.Media.PixelFormats.Bgra32, null, 0);
                             converted.CopyPixels(pixels, 4, 0);
                        }
                        else
                        {
                             cb.CopyPixels(pixels, 4, 0);
                        }

                        // BGRA
                        var c = System.Windows.Media.Color.FromRgb(pixels[2], pixels[1], pixels[0]);
                        vm.PickedColorHex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                    }
                    catch { }
                }
            }
        }
        else if (sender is System.Windows.Controls.Image img)
        {
             img.Cursor = System.Windows.Input.Cursors.Arrow;
        }
    }

    private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is ViewModels.ViewerViewModel vm && vm.IsColorPickerActive)
        {
            // Copy to clipboard and maybe deactivate?
            if (!string.IsNullOrEmpty(vm.PickedColorHex))
            {
                System.Windows.Clipboard.SetText(vm.PickedColorHex);
                // Optional: Deactivate picker after pick
                // vm.IsColorPickerActive = false; 
                // Or just keep it active for multiple picks.
            }
        }
    }
}
