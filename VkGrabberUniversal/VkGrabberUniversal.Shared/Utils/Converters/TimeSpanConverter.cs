using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace VkGrabberUniversal.Utils.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value ?? new TimeSpan();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}