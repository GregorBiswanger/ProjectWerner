using ProjectWerner.Face2Speech.Models;
using ProjectWerner.Face2Speech.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectWerner.Face2Speech.Functions
{
    [Export(typeof(IDictionaryManager))]
    public class DictionaryManager : IDictionaryManager
    {
        private const string WordsTempFileName = "WordsTemp.xml";

        public ObservableCollection<LanguageDictionary> AllLanguages { get; set; } = new ObservableCollection<LanguageDictionary>();

        public void LoadAllWords(CultureInfo selectedCulture)
        {
            Deserialize();
            var languageExists = AllLanguages.Any(language => language.Language == selectedCulture.Name);

            if (!languageExists)
            {
                LoadFromFile(selectedCulture);
                Serialize();
            }
        }

        public void OnExit()
        {
            Serialize();
        }

        public void SaveCallsNextWords(string DisplayText)
        {
            string[] allWords = DisplayText.Split(' ');
            for (int i= 0; i < allWords.Length;i++)
            {
                var proposalWords = AllLanguages[0].Words.Where(MyText => MyText.Text.Equals(allWords[i])).FirstOrDefault<WordDictionary>();
                if (proposalWords != null)
                {
                    proposalWords.Calls += 1;
                    if (i < allWords.Length - 1)
                    {
                        var nextWordsOriginal = AllLanguages[0].Words.Where(MyText => MyText.Text.Equals(allWords[i + 1])).FirstOrDefault<WordDictionary>();
                        if (nextWordsOriginal != null)
                        {
                            nextWordsOriginal.Calls += 1;
                        }
                        

                        var nextWords = proposalWords.NextWords.Where(MyText => MyText.Text.Equals(allWords[i + 1])).FirstOrDefault<WordDictionary>();
                        if (nextWords == null)
                        {
                            WordDictionary newWordDictionary = new WordDictionary();
                            newWordDictionary.Text = allWords[i + 1];
                            newWordDictionary.Calls = 1;
                            proposalWords.NextWords.Add(newWordDictionary);
                        }
                        else
                        {
                            nextWords.Calls += 1;
                        }
                    }
                }
            }
        }
  
        private void LoadFromFile(CultureInfo selectedCulture)
        {
            ObservableCollection<WordDictionary> MyReturn = new ObservableCollection<WordDictionary>();

            string[] allLines = File.ReadAllLines("Extensions\\Face2Speech\\Dictionary\\words_" + selectedCulture.Name + ".txt");
            WordDictionary newWordDictionary;

            foreach (string line in allLines)
            {
                newWordDictionary = new WordDictionary();
                newWordDictionary.Text = line.ToUpper();
                newWordDictionary.Calls = 0;
                MyReturn.Add(newWordDictionary);
            }

            MyReturn = new ObservableCollection<WordDictionary>(MyReturn.Distinct());

            AllLanguages.Add(new LanguageDictionary()
            {
                Language = selectedCulture.Name,
                Words = MyReturn
            });
        }

        public ObservableCollection<Line> LoadKeyboardDictionary(CultureInfo selectedCulture)
        {
            ObservableCollection<Line> myReturnCollection = new ObservableCollection<Line>();
            Line _readDictLine;
            System.IO.StreamReader xmlstream;
            try
            {
                xmlstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\keyboard_" + selectedCulture.Name + ".xml", System.Text.Encoding.UTF8);
            }
            catch (Exception)
            {

                xmlstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\keyboard_de-DE.xml", System.Text.Encoding.UTF8);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlstream);

            foreach (XmlNode LineNode in xmlDoc.ChildNodes)
            {
                if (LineNode.Name == "Keyboard")
                {
                    foreach (XmlNode WordNode in LineNode.ChildNodes)
                    {
                        if (WordNode.Name == "Line")
                        {
                            _readDictLine = new Line();
                            foreach (XmlNode CharNode in WordNode.ChildNodes)
                            {
                                if (CharNode.InnerText != "")
                                {
                                    String myType = "";
                                    foreach (XmlAttribute CharNodeAttribute in CharNode.Attributes)
                                    {
                                        if (CharNodeAttribute.Name == "Type")
                                        {
                                            myType = CharNodeAttribute.Value;
                                        }
                                    }
                                    _readDictLine.Words.Add(new KeyboardChars { Text = CharNode.InnerText, Type = myType });
                                }
                            }
                            myReturnCollection.Add(_readDictLine);
                        }
                    }
                }
            }
            return myReturnCollection;
        }

        private void Serialize()
        {

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<LanguageDictionary>));
            using (TextWriter writer = new StreamWriter(WordsTempFileName))
            {
                serializer.Serialize(writer, AllLanguages);
            }
        }

        private void Deserialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<LanguageDictionary>));
            if (File.Exists(WordsTempFileName))
            {
                using (TextReader reader = new StreamReader(WordsTempFileName))
                {
                    AllLanguages = (ObservableCollection<LanguageDictionary>)serializer.Deserialize(reader);
                }
            }
        }
    }
}