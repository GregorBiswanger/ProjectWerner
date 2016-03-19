using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectWerner.Dto;
using ProjectWerner.MvvmHelper.ViewModelBase;

namespace ProjectWerner.ViewModels.MainWindow
{
	internal interface IMainWindowViewModel : IViewModel
	{	
		ICommand ExecuteExtension { get; }

		ObservableCollection<ExtensionDataSet> Extensions { get; } 
	}
}