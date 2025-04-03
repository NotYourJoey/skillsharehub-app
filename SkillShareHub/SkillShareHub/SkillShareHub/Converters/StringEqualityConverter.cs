using System;
using System.Globalization;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class StringEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string compareValue)
            {
                bool areEqual = string.Equals(stringValue, compareValue, StringComparison.OrdinalIgnoreCase);

                // Check if we have a secondary parameter for color values
                var param2 = parameter as string;
                if (param2 != null && param2.Contains(","))
                {
                    string[] colors = param2.Split(',');
                    if (colors.Length == 2)
                    {
                        return areEqual ? Color.FromHex(colors[0]) : Color.FromHex(colors[1]);
                    }
                }

                // Default to returning the equality result
                return areEqual ? "#3897F0" : "#F0F0F0";
            }

            return "#F0F0F0"; // Default background color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // Not needed for one-way binding
        }
    }
}
