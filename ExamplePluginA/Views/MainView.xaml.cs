using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ExamplePluginA.Views
{
	public partial class MainView : IAppExtension
    {
        public MainView()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
