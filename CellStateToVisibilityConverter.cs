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
            if (value is CellState state && parameter is string param)
            {
                return (state, param) switch
                {
                    (CellState.Pending, "ShowTick") => Visibility.Visible,
                    (CellState.Pending, "ShowX") => Visibility.Visible,
                    (CellState.Accepted, "ShowTick") => Visibility.Collapsed,
                    (CellState.Accepted, "ShowX") => Visibility.Visible,
                    (CellState.Rejected, "ShowTick") => Visibility.Visible,
                    (CellState.Rejected, "ShowX") => Visibility.Collapsed,
                    _ => Visibility.Visible
                };
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}