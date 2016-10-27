using System.Windows;
using ProjectWerner.Features.ExtensionDashboard;

namespace ProjectWerner.Services
{
	internal static class ExtensionStarter
	{
		public static void StartExtension(FrameworkElement extensionMainElement)
		{			
			var extensionWindow = new ExtensionDashboardView
			{
				LayoutRoot = {Child = extensionMainElement},
				Height = extensionMainElement.Height,
				Width = extensionMainElement.Width
			};

			extensionWindow.Show();
		}
	}
}
