using ProjectWerner.Face2Speech.Models;
using ProjectWerner.Face2Speech.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectWerner.Face2Speech.Functions
{
    class DictionaryManager
    {
        //public ObservableCollection<Words> LoadWordsDictionary(String selectedCulture)
        //{
        //    ObservableCollection<Words> myReturnCollection = new ObservableCollection<Words>();
        //    String myLine;
        //    String myText;
        //    String[] myWords;
        //    Words myWordsObject;

        //    System.IO.StreamReader textstream;

        //    try
        //    {
        //        textstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\words_" + selectedCulture + ".txt", System.Text.Encoding.UTF8);
        //    }
        //    catch (Exception)
        //    {

        //        textstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\words_de-DE.txt", System.Text.Encoding.UTF8);
        //    }

        //    while (textstream.Peek() >= 0)
        //    {
        //        myLine = textstream.ReadLine();
        //        myText = myLine.ToUpper();
        //        myWordsObject = new Words();
        //        myWordsObject.NextWords = new List<String>();

        //        if (myLine.IndexOf(";") > 0)
        //        {
        //            //Wenn ; vorhanden, dann sind auch sinnvolle darauffolgende Wörter vorhanden, die gespeichert werden.                    
        //            myWords = myLine.Split(new Char[] { ';' });

        //            for (int i = 0; i < myWords.Length; i++)
        //            {
        //                myText = myWords[i].ToUpper();
        //                if (i == 0)
        //                {
        //                    myWordsObject.Text = myText;
        //                }
        //                else
        //                {
        //                    myWordsObject.NextWords.Add(myWords[i].ToUpper());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            myWordsObject.Text = myText;

        //        }
        //        myReturnCollection.Add(myWordsObject);
        //    }
        //    return myReturnCollection;
        //}

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
            using (TextWriter writer = new StreamWriter("MeineDatei.xml"))
            {
                serializer.Serialize(writer, AllLanguages);
            }
        }

        private void Deserialize()
        {

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<LanguageDictionary>));
            if (File.Exists("MeineDatei.xml") ){
                using (TextReader reader = new StreamReader("MeineDatei.xml"))
                {
                    AllLanguages = (ObservableCollection<LanguageDictionary>)serializer.Deserialize(reader);
                }
            }
       }
    }

}

