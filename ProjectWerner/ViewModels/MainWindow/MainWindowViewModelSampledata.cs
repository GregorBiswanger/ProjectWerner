using System.ComponentModel;
using System.Windows.Input;

namespace ProjectWerner.ViewModels.MainWindow
{
	internal class MainWindowViewModelSampledata : IMainWindowViewModel
	{
		public ICommand StartFaceToSpeach => null;

		public void Dispose () { }
		public event PropertyChangedEventHandler PropertyChanged;		
	}
}