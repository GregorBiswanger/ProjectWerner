using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWerner.Dto;

#pragma warning disable 0067

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModelSampledata : IMainWindowViewModel
	{
		private static ImageSource GetDefaultIcon()
		{
			return new BitmapImage(new Uri("pack://application:,,,/ProjectWerner;component/Images/default-icon.png"));
		}

		public MainWindowViewModelSampledata()
		{			
			Extensions = new ObservableCollection<ExtensionDataSet>
			{
				new ExtensionDataSet(null, "Ex1", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex2", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex3", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex4", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex5", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex6", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex7", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex8", GetDefaultIcon(), Guid.Empty),
				new ExtensionDataSet(null, "Ex9", GetDefaultIcon(), Guid.Empty),
			};

			Extensions.First().IsSelected = true;
		}

		public ICommand ExecuteExtension { get {return null; } }

        public ObservableCollection<ExtensionDataSet> Extensions { get; private set; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}