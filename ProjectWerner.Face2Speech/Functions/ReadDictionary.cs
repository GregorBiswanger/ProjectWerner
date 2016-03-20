using ProjectWerner.Face2Speech.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectWerner.Face2Speech.Functions
{
    class ReadDictionary
    {
        public ObservableCollection<Words> LoadWordsDictionary(String selectedCulture)
        {
            ObservableCollection<Words> myReturnCollection = new ObservableCollection<Words>();
            String myLine;
            String myText;
            String[] myWords;
            Words myWordsObject;

            System.IO.StreamReader textstream;

            try
            {
                textstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\words_" + selectedCulture + ".txt", System.Text.Encoding.UTF8);
            }
            catch (Exception)
            {

                textstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\words_de-DE.txt", System.Text.Encoding.UTF8);
            }

            while (textstream.Peek() >= 0)
            {
                myLine = textstream.ReadLine();
                myText = myLine.ToUpper();
                myWordsObject = new Words();
                myWordsObject.NextWords = new List<String>();

                if (myLine.IndexOf(";") > 0)
                {
                    //Wenn ; vorhanden, dann sind auch sinnvolle darauffolgende Wörter vorhanden, die gespeichert werden.                    
                    myWords = myLine.Split(new Char[] { ';' });

                    for (int i = 0; i < myWords.Length; i++)
                    {
                        myText = myWords[i].ToUpper();
                        if (i == 0)
                        {
                            myWordsObject.Text = myText;
                        }
                        else
                        {
                            myWordsObject.NextWords.Add(myWords[i].ToUpper());
                        }
                    }
                }
                else
                {
                    myWordsObject.Text = myText;

                }
                myReturnCollection.Add(myWordsObject);
            }
            return myReturnCollection;
        }

        public ObservableCollection<Line> LoadKeyboardDictionary(String selectedCulture)
        {
            ObservableCollection<Line> myReturnCollection = new ObservableCollection<Line>();
            Line _readDictLine;
            System.IO.StreamReader xmlstream;
            try
            {
                xmlstream = new System.IO.StreamReader("Extensions\\Face2Speech\\Dictionary\\keyboard_" + selectedCulture + ".xml", System.Text.Encoding.UTF8);
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
    }
}
