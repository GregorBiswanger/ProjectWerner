using System.ComponentModel;
using ProjectWerner.MvvmHelper.Utils;

namespace ProjectWerner.MvvmHelper.ViewModelBase
{
	public abstract class ViewModel : DisposingObject, 
                                      IViewModel                                      
    {
        public abstract event PropertyChangedEventHandler PropertyChanged;
    }
}