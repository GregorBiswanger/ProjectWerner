using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.ExampleExenstionB.Views
{
    /// <summary>
    /// Interaktionslogik für MainView.xaml
    /// </summary>
    public partial class MainView : IAppExtension
    {
        public MainView()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
