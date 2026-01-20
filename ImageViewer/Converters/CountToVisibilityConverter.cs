using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageViewer.Converters;

public class CountToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int count = 0;
        if (value is int i) count = i;
        
        // Default: 0 count -> Visible (Show Welcome), >0 -> Collapsed
        // Inverse: 0 count -> Collapsed, >0 -> Visible (Show Gallery)
        
        bool isZero = count == 0;
        
        if (Inverse)
        {
            return isZero ? Visibility.Collapsed : Visibility.Visible;
        }
        else
        {
            return isZero ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
