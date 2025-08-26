using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomGridControl
{
    public class CellTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CellType cellType && parameter is string param)
            {
                return (cellType, param) switch
                {
                    (CellType.Editable, "Editable") => Visibility.Visible,
                    (CellType.ReadOnly, "ReadOnly") => Visibility.Visible,
                    _ => Visibility.Collapsed
                };
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}