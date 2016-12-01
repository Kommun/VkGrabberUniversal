using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace VkGrabberUniversal.Utils.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value ?? DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value ?? DateTime.Now;
        }
    }
}
