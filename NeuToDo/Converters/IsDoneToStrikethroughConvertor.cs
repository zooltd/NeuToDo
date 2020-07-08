using System;
using System.Globalization;
using Xamarin.Forms;

namespace NeuToDo.Converters
{
    public class IsDoneToStrikethroughConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? TextDecorations.Strikethrough : TextDecorations.None;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => TextDecorations.None;
    }
}