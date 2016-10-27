using System.Windows;
using ProjectWerner.Features.ExtensionWindow;

namespace ProjectWerner.Services
{
	internal static class ExtensionStarter
	{
		public static void StartExtension(FrameworkElement extensionMainElement)
		{			
			var extensionWindow = new ExtensionWindowView
			{
				LayoutRoot = {Child = extensionMainElement},
				Height = extensionMainElement.Height,
				Width = extensionMainElement.Width
			};

			extensionWindow.Show();
		}
	}
}
