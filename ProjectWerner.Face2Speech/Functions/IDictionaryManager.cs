using ProjectWerner.Face2Speech.Models;
using ProjectWerner.Face2Speech.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWerner.Face2Speech.Functions
{
    public interface IDictionaryManager
    {
        ObservableCollection<LanguageDictionary> AllLanguages { get; set; }

        void LoadAllWords(CultureInfo selectedCulture);

        void OnExit();

        void SaveCallsNextWords(string DisplayText);


        ObservableCollection<Line> LoadKeyboardDictionary(CultureInfo selectedCulture);


    }
}
