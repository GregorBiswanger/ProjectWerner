using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.Notepad.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    [AppExtensionMetadata(Name = "Notepad")]
    [Export(typeof(IAppExtension))]
    public partial class MainView : IAppExtension
    {
        public MainView()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl => this;
        public void OnApplicationClosed()
        {
            
        }
    }
}
