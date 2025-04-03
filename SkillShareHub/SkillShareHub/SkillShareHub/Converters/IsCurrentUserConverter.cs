using System;
using System.Globalization;
using SkillShareHub.Helpers;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class IsCurrentUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int userId)
            {
                return userId == Settings.UserId;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
