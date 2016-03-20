using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;

namespace ProjectWerner.Extensions.HelloWorld.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl, IAppExtension
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public UserControl AppUserControl { get { return this; } }
    }
}
