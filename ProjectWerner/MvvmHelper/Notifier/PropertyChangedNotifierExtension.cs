using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectWerner.MvvmHelper.Notifier
{
	public static class PropertyChangedNotifierExtension
	{
		public static void ChangeAndNotify<T> (this PropertyChangedEventHandler handler, object sender,
								   ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return;

			field = value;

			if (handler != null)
                handler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
		}

		public static void Notify (this PropertyChangedEventHandler handler, object sender, string propertyName)
		{
			if (handler != null)
                handler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
		}
	}
}
