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
using ProjectWerner.EmailClient.ViewModels;

namespace ProjectWerner.EmailClient.Views
{
    /// <summary>
    /// Interaction logic for ChooseActionView.xaml
    /// </summary>
    [AppExtensionMetadata(Name = "MailClient")]
    [Export(typeof(IAppExtension))]
    public partial class ChooseActionView : UserControl, IAppExtension
    {
        private MainViewModel viewModel = new MainViewModel();

        public ChooseActionView()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public UserControl AppUserControl => this;

        public void OnApplicationClosed()
        {

        }
    }
}
