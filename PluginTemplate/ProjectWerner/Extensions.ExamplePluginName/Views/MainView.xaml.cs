using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace $safeprojectname$.Views
{
	[AppExtensionMetadataAttribute(Name = "<Your plugin Name>")]
	[Export(typeof(IAppExtension))]
	public partial class MainView : IAppExtension
    {
        public MainView()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
