using System;
using System.Globalization;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class DateTimeToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var now = DateTime.UtcNow;
                var timeSpan = now - dateTime;

                if (timeSpan.TotalSeconds < 60)
                {
                    return "just now";
                }
                if (timeSpan.TotalMinutes < 60)
                {
                    var minutes = (int)timeSpan.TotalMinutes;
                    return $"{minutes} {(minutes == 1 ? "minute" : "minutes")} ago";
                }
                if (timeSpan.TotalHours < 24)
                {
                    var hours = (int)timeSpan.TotalHours;
                    return $"{hours} {(hours == 1 ? "hour" : "hours")} ago";
                }
                if (timeSpan.TotalDays < 7)
                {
                    var days = (int)timeSpan.TotalDays;
                    return $"{days} {(days == 1 ? "day" : "days")} ago";
                }
                if (timeSpan.TotalDays < 30)
                {
                    var weeks = (int)(timeSpan.TotalDays / 7);
                    return $"{weeks} {(weeks == 1 ? "week" : "weeks")} ago";
                }
                if (timeSpan.TotalDays < 365)
                {
                    var months = (int)(timeSpan.TotalDays / 30);
                    return $"{months} {(months == 1 ? "month" : "months")} ago";
                }

                var years = (int)(timeSpan.TotalDays / 365);
                return $"{years} {(years == 1 ? "year" : "years")} ago";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
