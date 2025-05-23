using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.UILayer.Converters
{
    /// <summary>
    /// Zeigt "Yes" für true, "No" für false, "–" für null (nicht gesetzt).
    /// </summary>
    public class ChildFriendlinessToTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] is bool? ChildFriendliness
            if (values.Length == 0 || values[0] == null || values[0] == System.Windows.DependencyProperty.UnsetValue)
                return "–";

            if (values[0] is bool b)
                return b ? "Yes" : "No";

            return "–";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}