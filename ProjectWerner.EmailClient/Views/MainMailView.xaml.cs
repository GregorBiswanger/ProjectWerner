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
    /// Interaction logic for MainMailView.xaml
    /// </summary>
    [AppExtensionMetadata(Name = "MailClient")]
    [Export(typeof (IAppExtension))]
    public partial class MainMailView : IAppExtension
    {
        public MainMailView()
        {
            InitializeComponent();

            ((UserControl) this).Loaded += (sender, args) =>
            {
                MainViewModel xxx = (MainViewModel) AppUserControl.DataContext;
                
                xxx.Add("#AN");
                xxx.Add("hjst@gmx.de");
                xxx.Add("#NaChriCht");
                xxx.Add("Hi,");
                xxx.Add("Gesendet");
                xxx.Add("mit");
                xxx.Add("OpenSoftware");
                xxx.Add("App");
                xxx.Add("For");
                xxx.Add("ProjectWerner");
                xxx.Add("#bEtreFF");
                xxx.Add("betreff");
                //xxx.Add("#sende");

                xxx.ShowWindow(MainViewModel.MailWindow.WriteMail);

                this.Control.Content = xxx.Content;
            };

        }

        
        public UserControl AppUserControl => this;
        public void OnApplicationClosed()
        {
            
        }
    }
}
