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
    public class LanguageDictionary
    {
        public string Language { get; set; }
        public ObservableCollection<WordDictionary> Words { get; set; }

    }
}
