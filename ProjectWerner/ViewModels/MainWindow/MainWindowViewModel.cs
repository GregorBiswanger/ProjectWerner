using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ProjectWerner.Dto;
using ProjectWerner.MvvmHelper.Commands;
using ProjectWerner.MvvmHelper.ViewModelBase;
using ProjectWerner.Services;
using ProjectWerner.Views;

#pragma warning disable 0067

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModel : ViewModel, IMainWindowViewModel
	{		
		public MainWindowViewModel(IExtensionLoader extensionLoader)
		{
			Extensions = new ObservableCollection<ExtensionDataSet>(extensionLoader.GetExtensions());
			Extensions.First().IsSelected = true;


			ExecuteExtension = new ParameterrizedCommand<ExtensionDataSet>(
				extensionData =>
				{
					var extension = extensionData.Extension;

					var extensionWindow = new ExtensionWindow();
					extensionWindow.LayoutRoot.Children.Add(extension.AppUserControl);
					extensionWindow.Height = extension.AppUserControl.Height;
					extensionWindow.Width = extension.AppUserControl.Width;
					extensionWindow.Show();
				}	
			);
		}

		public ICommand ExecuteExtension { get; }

		public ObservableCollection<ExtensionDataSet> Extensions { get; }

		protected override void CleanUp() {	}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
