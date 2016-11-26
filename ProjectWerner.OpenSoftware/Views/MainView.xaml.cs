using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Win32;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.OpenSoftware.ViewModels;

namespace ProjectWerner.OpenSoftware.Views
{
	[AppExtensionMetadata(Name = "OpenSoftware")]	
	[Export(typeof(IAppExtension))]
	public partial class MainView : IAppExtension
    {
        public MainView()
        {
            InitializeComponent();
           
            ((UserControl) this).Loaded += (sender, args) =>
            {
                MainViewModel xxx = (MainViewModel)AppUserControl.DataContext;
                xxx.Add("gimp");
            };

        }

        
        public UserControl AppUserControl => this;
	    public void OnApplicationClosed()
	    {
	        
	    }
    }
}
