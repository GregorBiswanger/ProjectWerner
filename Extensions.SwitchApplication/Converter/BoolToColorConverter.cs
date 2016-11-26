using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectWerner.SwitchApplication.Converter
{
    /// <summary>
    /// Used for converting Boolean to color and back. 
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        private readonly SolidColorBrush defaultColor = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        private readonly SolidColorBrush activeColor = Brushes.Yellow;

        /// <summary>
        /// Convert a Boolean to its color equal.
        /// </summary>
        /// <param name="value">The Boolean.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The color.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return defaultColor;
            return (bool)value ? activeColor : defaultColor;
        }

        /// <summary>
        /// Convert back a Color to its Boolean equal.
        /// </summary>
        /// <param name="value">The Color.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The Boolean.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            if ((SolidColorBrush)value == activeColor)
            {
                return true;
            }
            else if (((SolidColorBrush)value == defaultColor))
            {
                return false;
            }
            return false;
        }
    }
}
