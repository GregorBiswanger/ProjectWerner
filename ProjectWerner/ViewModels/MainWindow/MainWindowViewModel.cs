using System.ComponentModel;
using System.Windows.Input;
using ProjectWerner.MvvmHelper.Commands;
using ProjectWerner.MvvmHelper.ViewModelBase;
using ProjectWerner.Services;
using ProjectWerner.Views;

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModel : ViewModel, IMainWindowViewModel
	{		
		public MainWindowViewModel(ExtensionLoader extensionLoader)
		{
			
			StartFaceToSpeach = new Command(
				() =>
				{
					var faceToSpeachExtension = extensionLoader.GetFaceToSpeachExtension();

					var extensionWindow = new ExtensionWindow();
					extensionWindow.LayoutRoot.Children.Add(faceToSpeachExtension.AppUserControl);
					extensionWindow.Height = faceToSpeachExtension.AppUserControl.Height;
					extensionWindow.Width = faceToSpeachExtension.AppUserControl.Width;
					extensionWindow.Show();
				}	
			);
		}

		public ICommand StartFaceToSpeach { get; }

		protected override void CleanUp() {	}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
