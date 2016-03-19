using System.Windows.Input;
using ProjectWerner.MvvmHelper.ViewModelBase;

namespace ProjectWerner.ViewModels.MainWindow
{
	internal interface IMainWindowViewModel : IViewModel
	{	
		ICommand StartFaceToSpeach { get; }	
	}
}