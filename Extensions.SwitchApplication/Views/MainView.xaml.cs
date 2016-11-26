using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.SwitchApplication.Views
{
    [AppExtensionMetadata(Name = "<Your plugin Name>")]
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
