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

#pragma warning disable 0067

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModel : ViewModel, IMainWindowViewModel
	{
		private readonly DispatcherTimer selectionTimer;
		private int selectedItem;

		public MainWindowViewModel(IExtensionLoader extensionLoader)
		{
			selectedItem = 0;
			selectionTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(2)				
			};
			selectionTimer.Start();			

			selectionTimer.Tick += OnSeletionTimerTick;

			Extensions = new ObservableCollection<ExtensionDataSet>(extensionLoader.GetExtensions());
			Extensions.First().IsSelected = true;

			ExecuteExtension = new Command(
				() => ExtensionStarter.StartExtension(Extensions[selectedItem].Extension.AppUserControl)	
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

		protected override void CleanUp()
		{
			selectionTimer.Stop();
			selectionTimer.Tick -= OnSeletionTimerTick;
		}
		public override event PropertyChangedEventHandler PropertyChanged;		
	}
}
