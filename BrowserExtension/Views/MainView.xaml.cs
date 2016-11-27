using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;
using BrowserExtension.ViewModels;
using System.Windows;
using System;

namespace BrowserExtension.Views
{

	[AppExtensionMetadataAttribute(Name = "BrowserExtension")]	
	[Export(typeof(IAppExtension))]
	public partial class MainView : IAppExtension
    {
        private MainViewModel viewModel = new MainViewModel();
        
        public MainView()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public UserControl AppUserControl { get { return this; } }

        public void OnApplicationClosed()
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.OnLoaded();
        }
    }
}
