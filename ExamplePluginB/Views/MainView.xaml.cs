using System.ComponentModel.Composition;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.ExampleExenstionB.Views
{
	[AppExtensionMetadataAttribute(Name = "B")]
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
