using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using ProjectWerner.Contracts.Extensions;
using ProjectWerner.SwitchApplication.ViewModels;

namespace ProjectWerner.SwitchApplication.Views
{
    [AppExtensionMetadata(Name = "Switch application")]
    [Export(typeof(IAppExtension))]
    public partial class MainView : IAppExtension
    {
        private readonly MainViewModel mainViewModel = new MainViewModel();
        public MainView()
        {
            InitializeComponent();
            DataContext = mainViewModel;
            foreach (var buttonItem in mainViewModel.Buttons)
            {
                StackPanel.Children.Add(new Button
                {
                    Content = buttonItem.Label,
                    Command = buttonItem.Command,
                    Margin = new Thickness { Bottom = 5d, Left = 5d, Right = 5d, Top = 5d }
                });
            }
        }

        public UserControl AppUserControl => this;
    }
}
