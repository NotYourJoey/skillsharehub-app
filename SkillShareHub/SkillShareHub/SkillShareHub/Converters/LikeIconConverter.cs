using System;
using System.Globalization;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class LikeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLiked)
            {
                return isLiked ? "like_filled_icon.png" : "like_icon.png";
            }
            return "like_icon.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
