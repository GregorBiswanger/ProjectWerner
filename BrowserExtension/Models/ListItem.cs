using BrowserExtension.JsonModels;
using PropertyChanged;
using System.Windows.Input;

namespace BrowserExtension.Models
{
    [ImplementPropertyChanged]
    public class ListItem
    {
        public ListItem()
        {
        }

        public value SearchResult { get; set; }

        public bool IsActive { get; set; }

        public ICommand Command { get; set; }
    }
}
