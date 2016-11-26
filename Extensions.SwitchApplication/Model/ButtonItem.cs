using System.Windows.Input;
using ProjectWerner.SwitchApplication.Interfaces;

namespace ProjectWerner.SwitchApplication.Model
{
    public class ButtonItem
    {
        public string Label { get; set; }

        public bool IsActive { get; set; }

        public IProcessInfo ProcessInfo { get; set; }

        public ICommand Command { get; set; }
    }
}
