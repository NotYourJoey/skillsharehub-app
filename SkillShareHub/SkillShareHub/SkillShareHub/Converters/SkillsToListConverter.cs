using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class SkillsToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string skillsString && !string.IsNullOrEmpty(skillsString))
            {
                return skillsString.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }

            return new List<string>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> skills)
            {
                return string.Join(", ", skills);
            }

            return string.Empty;
        }
    }
}
