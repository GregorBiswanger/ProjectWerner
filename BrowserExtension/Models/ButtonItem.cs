using PropertyChanged;
using System.Windows.Input;

namespace BrowserExtension.Models
{
    [ImplementPropertyChanged]
    public class ButtonItem
    {
        public ButtonItem()
        {
        }

        public string Label { get; set; }

        public bool IsActive { get; set; }

        public ICommand Command { get; set; }
    }
}
