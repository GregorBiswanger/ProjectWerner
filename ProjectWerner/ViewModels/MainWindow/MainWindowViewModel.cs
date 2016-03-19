using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
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
		private DispatcherTimer selectionTimer;
		private int selectedItem;

		public MainWindowViewModel(IExtensionLoader extensionLoader)
		{
			selectedItem = 0;
			selectionTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(2),
				IsEnabled = true
			};

			selectionTimer.Tick += OnSeletionTimerTick;

			Extensions = new ObservableCollection<ExtensionDataSet>(extensionLoader.GetExtensions());
			Extensions.First().IsSelected = true;


			ExecuteExtension = new Command(
				() =>
				{
					var extension = Extensions[selectedItem].Extension;

					var extensionWindow = new ExtensionWindow();
					extensionWindow.LayoutRoot.Children.Add(extension.AppUserControl);
					extensionWindow.Height = extension.AppUserControl.Height;
					extensionWindow.Width = extension.AppUserControl.Width;
					extensionWindow.Show();
				}	
			);
		}

		private void OnSeletionTimerTick(object sender, EventArgs eventArgs)
		{
			Extensions[selectedItem].IsSelected = false;

			selectedItem++;

			if (selectedItem == Extensions.Count)
				selectedItem = 0;

			Extensions[selectedItem].IsSelected = true;
		}

		public ICommand ExecuteExtension { get; }

		public ObservableCollection<ExtensionDataSet> Extensions { get; }

		protected override void CleanUp() {	}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
