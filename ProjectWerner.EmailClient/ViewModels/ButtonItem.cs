using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace ProjectWerner.EmailClient.ViewModels
{
    [ImplementPropertyChanged]
    public class ButtonItem
        {
            public ButtonItem()
            {
            }

            public string Label { get; set; }

            public bool IsActive { get; set; }

         
        }
}