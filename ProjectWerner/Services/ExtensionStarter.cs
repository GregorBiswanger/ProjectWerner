using System.Windows;
using ProjectWerner.Views;

namespace ProjectWerner.Services
{
	internal static class ExtensionStarter
	{
		public static void StartExtension(FrameworkElement extensionMainElement)
		{			
			var extensionWindow = new ExtensionWindow
			{
				LayoutRoot = {Child = extensionMainElement},
				Height = extensionMainElement.Height,
				Width = extensionMainElement.Width
			};

			extensionWindow.Show();
		}
	}
}
