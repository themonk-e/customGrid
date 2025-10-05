using System;
using System.Globalization;
using System.Windows.Data;
using CustomGridControl.Models;

namespace CustomGridControl.Converters
{
    /// <summary>
    /// Returns the "old" value text based on cell state
    /// - Pending/Accepted: Returns DestValue (old value from destination)
    /// - Rejected: Returns SourceValue (new value becomes the "rejected" one)
    /// </summary>
    public class CellStateToOldValueConverter : IMultiValueConverter
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
                    CellState.Rejected => sourceValue, // When rejected, source (new) becomes the strikethrough value
                    _ => destValue // Pending or Accepted: dest (old) is strikethrough
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
