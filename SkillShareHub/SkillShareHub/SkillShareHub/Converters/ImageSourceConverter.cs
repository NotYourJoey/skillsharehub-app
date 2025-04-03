using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace SkillShareHub.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return parameter != null ? ImageSource.FromFile(parameter.ToString()) : null;
            }

            if (value is string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return parameter != null ? ImageSource.FromFile(parameter.ToString()) : null;
                }

                if (path.StartsWith("http://") || path.StartsWith("https://"))
                {
                    return ImageSource.FromUri(new Uri(path));
                }

                if (File.Exists(path))
                {
                    return ImageSource.FromFile(path);
                }

                return ImageSource.FromFile(path);
            }

            if (value is byte[] bytes)
            {
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }

            return parameter != null ? ImageSource.FromFile(parameter.ToString()) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // Not needed for one-way binding
        }
    }
}
