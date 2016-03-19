using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ProjectWerner.Dto;

#pragma warning disable 0067

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModelSampledata : IMainWindowViewModel
	{
		public MainWindowViewModelSampledata()
		{			
			Extensions = new ObservableCollection<ExtensionDataSet>
			{
				new ExtensionDataSet(null, "Ex1", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex2", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex3", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex4", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex5", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex6", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex7", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex8", null, Guid.Empty),
				new ExtensionDataSet(null, "Ex9", null, Guid.Empty),
			};
		}

		public ICommand ExecuteExtension => null;

		public ObservableCollection<ExtensionDataSet> Extensions { get; }

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}