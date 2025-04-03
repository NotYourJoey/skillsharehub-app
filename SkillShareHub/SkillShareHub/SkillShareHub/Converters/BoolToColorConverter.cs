using System;
using System.Globalization;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string colorParam)
            {
                var colors = colorParam.Split(',');
                if (colors.Length == 2)
                {
                    return boolValue ? Color.FromHex(colors[0]) : Color.FromHex(colors[1]);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
