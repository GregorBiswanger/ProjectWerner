using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectWerner.Features.FacePreviewWindow.Converters
{
    public class WindowStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
                return WindowState.Normal;
            }
            else
            {
                return WindowState.Minimized;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}