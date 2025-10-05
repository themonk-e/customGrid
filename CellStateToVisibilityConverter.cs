using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomGridControl
{
    public class CellStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Both buttons should always be visible when there are differences
            // This allows users to toggle their selection at any time before saving
            // The HasDifference binding on the buttons panel controls overall visibility
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}