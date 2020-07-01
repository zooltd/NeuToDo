using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;

namespace NeuToDo.Converters
{
    public class WeekNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is List<int> temp ? string.Join(",", temp) : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => string.Empty;
    }
}