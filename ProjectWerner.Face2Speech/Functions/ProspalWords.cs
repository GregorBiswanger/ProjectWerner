using ProjectWerner.Face2Speech.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWerner.Face2Speech.Functions
{
    class ProspalWords
    {
        public enum SearchType { All = 0, OnlyEqual = 1, OnlyStartsWith = 2};

        public ObservableCollection<Words> GetFirstLines(ObservableCollection<Words> AllWords, String LastWord, SearchType mySearchType)
        {
            ObservableCollection<Words> myReturnCollection = new ObservableCollection<Words>();
            Dictionary<String, String> addedWords = new Dictionary<string, string>();
            Words newWord;
            IEnumerable<Words> proposalWords;

            int number = 0;

            if (mySearchType == SearchType.All || mySearchType == SearchType.OnlyEqual)
            {
                proposalWords = AllWords.Where(myWord => myWord.Text.Equals(LastWord)).OrderBy(x => x.Text.Length).Take(10);

                proposalWords.ToList().ForEach(myWord =>
                {
                    if (!addedWords.ContainsKey(myWord.Text))
                    {
                        number = number + 1;
                        if (number <= 10)
                        {
                            if (number == 10)
                            {
                                number = 0;
                            }

                            addedWords.Add(myWord.Text, "");
                            newWord = new Words();
                            newWord.Text = string.Format("{0}: {1}", number, myWord.Text);
                            newWord.NextWords = myWord.NextWords;
                            myReturnCollection.Add(newWord);
                        }

                    }
                });
            }

            if (mySearchType == SearchType.All || mySearchType == SearchType.OnlyStartsWith)
            {

                proposalWords = AllWords.Where(myWord => myWord.Text.StartsWith(LastWord)).OrderBy(x => x.Text.Length).Take(10);

                proposalWords.ToList().ForEach(myWord =>
                {
                    if (!addedWords.ContainsKey(myWord.Text))
                    {
                        number = number + 1;
                        if (number <= 10)
                        {
                            if (number == 10)
                            {
                                number = 0;
                            }

                            addedWords.Add(myWord.Text, "");
                            newWord = new Words();
                            newWord.Text = string.Format("{0}: {1}", number, myWord.Text);
                            newWord.NextWords = myWord.NextWords;
                            myReturnCollection.Add(newWord);
                        }

                    }
                });
            }


            return myReturnCollection;
        }
    }
}
