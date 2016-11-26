using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Win32;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.OpenSoftware.Views
{
	[AppExtensionMetadata(Name = "OpenSoftware")]	
	[Export(typeof(IAppExtension))]
	public partial class MainView : IAppExtension
    {
        public MainView()
        {
           

            InitializeComponent();
            
        }

	 

        public UserControl AppUserControl => this;
    }
}
