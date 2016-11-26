using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;
using ProjectWerner.EmailClient.Views;
using PropertyChanged;

namespace ProjectWerner.EmailClient.ViewModels
{
    public class MainViewModel
    {
        public enum MailWindow
        {
            WriteMail,
            ReadMail
        };

        private readonly WriteMailView _writeMailView;

        public UserControl Content { get; set; }

        /// <summary>
        /// empty constructor lists applications
        /// </summary>
        public MainViewModel()
        {
           _writeMailView = new WriteMailView();
        }

        /// <summary>
        /// Send text to Open Software
        /// </summary>
        /// <param name="text"></param>
        public void Add(string text)
        {
            AnalyzeText(text);
        }


        public void ShowWindow(MailWindow window)
        {
            switch (window)
            {
                case MailWindow.WriteMail:
                    Content = _writeMailView;
                    break;
                case MailWindow.ReadMail:
                    //set read mail window
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(window), window, null);
            }
        }


        /// <summary>
        /// analyzes the text sent
        /// </summary>
        /// <param name="text"></param>
        private void AnalyzeText(string text)
        {
            //when text starts with # then go to some field
            if (text.StartsWith("#"))
            {
                text = text.ToLower();
                text = text.Remove(0, 1);

                _writeMailView.ChangeField(text);
            }
            else
            {
                _writeMailView.InsertText(text);
            }
        }
        
    }
}