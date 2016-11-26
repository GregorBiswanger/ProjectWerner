using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWerner.Face2Speech.Models
{
    [Serializable]
    public class WordDictionary
    {
        public string Text { get; set; }
        public int Calls { get; set; }
        public ObservableCollection<WordDictionary> NextWords { get; set; } = new ObservableCollection<WordDictionary>();

    }
}
