using System;
using System.Collections.Generic;
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

namespace ExamplePluginA.Views
{
    /// <summary>
    /// Interaktionslogik für MainView.xaml
    /// </summary>
  
    //TODO: Re-Add IAppExtension Interface when loading of multiple plugin files work
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
