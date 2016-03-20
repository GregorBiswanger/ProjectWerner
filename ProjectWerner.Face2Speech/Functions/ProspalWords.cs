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
        public enum SearchType { All = 0, OnlyEqual = 1, OnlyStartsWith = 2 };
        public int Number = 0;

        public ObservableCollection<Words> GetFirstLines(ObservableCollection<Words> AllWords, String LastWord, SearchType mySearchType)
        {
            ObservableCollection<Words> myReturnCollection = new ObservableCollection<Words>();
            Dictionary<String, Words> addedWords = new Dictionary<string, Words>();
            Words newWord;
            IEnumerable<Words> proposalWords;

            if (mySearchType == SearchType.All || mySearchType == SearchType.OnlyEqual)
            {
                proposalWords = AllWords.Where(myWord => myWord.Text.Equals(LastWord)).OrderBy(x => x.Text.Length).Take(10);

                proposalWords.ToList().ForEach(myWord =>
                {
                    if (!addedWords.ContainsKey(myWord.Text) && myReturnCollection.Count < 10)
                    {
                        Number = Number + 1;
                        if (Number <= 10)
                        {
                            if (Number == 10)
                            {
                                Number = 0;
                            }

                            addedWords.Add(myWord.Text, myWord);
                            newWord = new Words();
                            newWord.Text = string.Format("{0}: {1}", Number, myWord.Text);
                            newWord.NextWords = myWord.NextWords;
                            myReturnCollection.Add(newWord);
                        }
                    }
                });
                if (myReturnCollection.Count == 0)
                {
                    Number = Number + 1;
                    if (Number <= 10)
                    {
                        if (Number == 10)
                        {
                            Number = 0;
                        }
                        newWord = new Words();
                        newWord.Text = string.Format("{0}: {1}", Number, LastWord);
                        //newWord.NextWords = myWord.NextWords;
                        myReturnCollection.Add(newWord);
                    }
                }
            }

            if (mySearchType == SearchType.All || mySearchType == SearchType.OnlyStartsWith)
            {

                proposalWords = AllWords.Where(myWord => myWord.Text.StartsWith(LastWord)).OrderBy(x => x.Text.Length).Take(10);

                proposalWords.ToList().ForEach(myWord =>
                {
                    if (!addedWords.ContainsKey(myWord.Text) && myReturnCollection.Count < 10)
                    {
                        Number = Number + 1;
                        if (Number <= 10)
                        {
                            if (Number == 10)
                            {
                                Number = 0;
                            }

                            addedWords.Add(myWord.Text, myWord);
                            newWord = new Words();
                            newWord.Text = string.Format("{0}: {1}", Number, myWord.Text);
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
