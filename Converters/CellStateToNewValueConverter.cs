using System;
using System.Globalization;
using System.Windows.Data;
using CustomGridControl.Models;

namespace CustomGridControl.Converters
{
    /// <summary>
    /// Returns the "new" value text based on cell state
    /// - Pending/Accepted: Returns SourceValue (new value from source)
    /// - Rejected: Returns DestValue (old value is kept, so it's the "current" value)
    /// </summary>
    public class CellStateToNewValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is CellState state &&
                values[1] is string destValue &&
                values[2] is string sourceValue)
            {
                return state switch
                {
                    CellState.Rejected => destValue, // When rejected, dest (old) is the current value
                    _ => sourceValue // Pending or Accepted: source (new) is the current value
                };
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
