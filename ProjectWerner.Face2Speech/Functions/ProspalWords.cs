using ProjectWerner.Face2Speech.Models;
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
        public enum SearchType { All = 0, OnlyEqual = 1, OnlyStartsWith = 2, NextWord = 3 };

        public ObservableCollection<WordDictionary> GetFirstLines(ObservableCollection<WordDictionary> AllWords, String LastWord, SearchType mySearchType)
        {
            ObservableCollection<WordDictionary> myReturnCollection = new ObservableCollection<WordDictionary>();
            Dictionary<String, WordDictionary> addedWords = new Dictionary<string, WordDictionary>();
            WordDictionary newWord;
            IEnumerable<WordDictionary> proposalWords;

            if (mySearchType == SearchType.NextWord)
            {
                proposalWords = AllWords.Where(MyText => MyText.Text.Equals(LastWord)).OrderByDescending(x => x.Calls).ThenBy(x => x.Text.Length).Take(10);

                newWord = FindSuggests(myReturnCollection, addedWords, proposalWords);

                if (myReturnCollection.Count == 0)
                {
                    newWord = new WordDictionary();
                    newWord.Text = LastWord;
                    myReturnCollection.Add(newWord);
                }
            }

            if (mySearchType == SearchType.All || mySearchType == SearchType.OnlyStartsWith || mySearchType == SearchType.OnlyEqual)
            {
                proposalWords = AllWords.Where(myWord => myWord.Text.StartsWith(LastWord)).OrderByDescending(x => x.Calls).ThenBy(x => x.Text.Length).Take(10);
                newWord = FindSuggests(myReturnCollection, addedWords, proposalWords);
            }

            return myReturnCollection;
        }
        public void SetNumbers(ObservableCollection<WordDictionary> AllWords)
        {
            int Number = 0;
            foreach (WordDictionary word in AllWords)
            {
                Number = Number + 1;
                if (Number <= 10)
                {
                    if (Number == 10)
                    {
                        Number = 0;
                    }
                    word.Text = string.Format("{0}: {1}", Number, word.Text);
                }
            }
        }

        private WordDictionary FindSuggests(ObservableCollection<WordDictionary> myReturnCollection, Dictionary<string, WordDictionary> addedWords, IEnumerable<WordDictionary> proposalWords)
        {
            WordDictionary newWord  = new Models.WordDictionary();
            proposalWords.ToList().ForEach(myWord =>
            {
                if (!addedWords.ContainsKey(myWord.Text) && myReturnCollection.Count < 10)
                {
                    addedWords.Add(myWord.Text, myWord);
                    newWord = new WordDictionary();
                    newWord.Text = myWord.Text;
                    newWord.NextWords = myWord.NextWords;
                    newWord.Calls = myWord.Calls;
                    myReturnCollection.Add(newWord);
                }
            });
            return newWord;
        }
    }
}
