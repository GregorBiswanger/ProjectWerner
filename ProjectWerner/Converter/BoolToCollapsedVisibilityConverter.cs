using System.Globalization;
using System.Windows;
using ProjectWerner.MvvmHelper.ConverterBase;

namespace ProjectWerner.Converter
{
	public class BoolToCollapsedVisibilityConverter : GenericValueConverter<bool, Visibility>
    {
	    protected override Visibility Convert(bool value, CultureInfo culture)
	    {
		    return value ? Visibility.Visible : Visibility.Collapsed;
	    }	   
    }
}
