using System.Collections.Generic;
using PropertyChanged;

namespace ProjectWerner.Extensions.Face2Speech.ViewModels
{
    [ImplementPropertyChanged]
    public class Line
    {
        public int SelectedWordIndex { get; set; }

        public List<Word> Words { get; set; }
        public bool IsSelected { get; set; }

        public Line()
        {
            Words = new List<Word>();
            SelectedWordIndex = -1;
        }
    }
}