using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;
using BrowserExtension.ViewModels;
using System.Windows;

namespace BrowserExtension.Views
{

	[AppExtensionMetadataAttribute(Name = "BrowserExtension")]	
	[Export(typeof(IAppExtension))]
	public partial class MainView : IAppExtension
    {
        private MainViewModel viewModel = new MainViewModel(Clipboard.GetText());
        
        public MainView()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
