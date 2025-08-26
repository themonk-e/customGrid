using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CustomGridControl
{
    public class CellStateToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is CellState state && values[1] is CellType cellType)
            {
                // ReadOnly cells are always white
                if (cellType == CellType.ReadOnly)
                {
                    return new SolidColorBrush(Colors.White);
                }

                // Editable cells have different colors based on state
                return state switch
                {
                    CellState.Accepted => new SolidColorBrush(Colors.LightGreen),
                    CellState.Rejected => new SolidColorBrush(Colors.LightCoral),
                    _ => new SolidColorBrush(Color.FromRgb(255, 255, 224)) // Light yellow (pale yellow)
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}