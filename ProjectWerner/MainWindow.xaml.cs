using ProjectWerner.API;
using ProjectWerner.Contracts.API;
using ProjectWerner.Contracts.Extensions;
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
using ProjectWerner.Views;

namespace ProjectWerner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App.CompositionContainer.ComposeParts(this);
        }

        [Import]
        private IAppExtension appExtension;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ExtensionWindow extensionWindow = new ExtensionWindow();
            extensionWindow.LayoutRoot.Children.Add(appExtension.AppUserControl);
            //extensionWindow.Height = appExtension.AppUserControl.Height;
            //extensionWindow.Width = appExtension.AppUserControl.Width;
            extensionWindow.Show();
        }
    }
}
