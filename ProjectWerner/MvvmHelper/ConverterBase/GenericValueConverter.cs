using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectWerner.MvvmHelper.ConverterBase
{
	public abstract class GenericValueConverter <TFrom, TTo> : IValueConverter
    {
	    protected virtual TTo Convert(TFrom value, CultureInfo culture)
	    {
			throw new NotImplementedException();
	    }

	    protected virtual TFrom ConvertBack(TTo value, CultureInfo culture)
	    {
			throw new NotImplementedException();
	    }

	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
			if (value != null)
				if (value.GetType() != typeof (TFrom))		   
					if (!(value is TFrom))
						throw new ArgumentException("types are not matching: cannot convert from " + value.GetType() + " to " + typeof(TFrom));
		   
		    return Convert((TFrom)value, culture);			
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
			if (value != null)
				if (value.GetType() != typeof (TTo))
					if (!(value is TTo))
						throw new ArgumentException("types are not matching: cannot convert from " + value.GetType() + " to " + typeof(TTo));
		    
		    return ConvertBack((TTo) value, culture);			
	    }
    }
}
