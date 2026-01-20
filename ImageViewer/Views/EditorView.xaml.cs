using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using ImageViewer.ViewModels;

namespace ImageViewer.Views;

public partial class EditorView : System.Windows.Controls.UserControl
{
    private bool _isDragging = false;
    private System.Windows.Point _startPoint;
    private Rect _currentSelection;

    public EditorView()
    {
        InitializeComponent();
    }

    private void CropCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _startPoint = e.GetPosition(CropCanvas);
        SelectionRect.Visibility = Visibility.Visible;
        SelectionRect.Width = 0;
        SelectionRect.Height = 0;
        Canvas.SetLeft(SelectionRect, _startPoint.X);
        Canvas.SetTop(SelectionRect, _startPoint.Y);
        CropCanvas.CaptureMouse();
    }

    private void CropCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDragging) return;

        var currentPoint = e.GetPosition(CropCanvas);
        
        // Calculate rect
        double x = Math.Min(_startPoint.X, currentPoint.X);
        double y = Math.Min(_startPoint.Y, currentPoint.Y);
        double w = Math.Abs(currentPoint.X - _startPoint.X);
        double h = Math.Abs(currentPoint.Y - _startPoint.Y);

        _currentSelection = new Rect(x, y, w, h);

        Canvas.SetLeft(SelectionRect, x);
        Canvas.SetTop(SelectionRect, y);
        SelectionRect.Width = w;
        SelectionRect.Height = h;
    }

    private void CropCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        CropCanvas.ReleaseMouseCapture();
    }

    private void OnCropClicked(object sender, RoutedEventArgs e)
    {
        if (_currentSelection.Width > 0 && _currentSelection.Height > 0 && DataContext is EditorViewModel vm)
        {
            // Convert to Normalized Coordinates (0-1) based on the Image control actual size
            double normX = _currentSelection.X / ImgDisplay.ActualWidth;
            double normY = _currentSelection.Y / ImgDisplay.ActualHeight;
            double normW = _currentSelection.Width / ImgDisplay.ActualWidth;
            double normH = _currentSelection.Height / ImgDisplay.ActualHeight;

            try 
            {
               // Using command execution via ViewModel
               vm.CropCommand.Execute(new Rect(normX, normY, normW, normH));
               
               // Reset selection UI
               SelectionRect.Visibility = Visibility.Collapsed;
               _currentSelection = Rect.Empty;
               SelectionRect.Width = 0;
               SelectionRect.Height = 0;
            } 
            catch { }
        }
    }
}
