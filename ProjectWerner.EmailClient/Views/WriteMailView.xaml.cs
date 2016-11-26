using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectWerner.EmailClient.ViewModels;

namespace ProjectWerner.EmailClient.Views
{
    /// <summary>
    /// Interaction logic for WriteMailView.xaml
    /// </summary>
    public partial class WriteMailView : UserControl
    {
        private enum Field
        {
            Subject,
            To,
            Body,
        }

        private Field _insertField;
        public readonly MailItem Mail;

        public WriteMailView()
        {
            InitializeComponent();

            Mail = new MailItem();

            DataContext = this;
        }


        public void InsertText(string text)
        {
            switch (_insertField)
            {
                case Field.Subject:
                    Mail.Subject = text;
                    Subject.Text = Mail.Subject;
                    break;
                case Field.To:
                    Mail.To = text;
                    To.Text = Mail.To;
                    break;
                case Field.Body:
                    Mail.Body += " " + text;
                    Message.Text = Mail.Body;
                    break;
            }
        }

        public void ChangeField(string field)
        {
            switch (field)
            {
                case "an":
                    _insertField = Field.To;
                    break;
                case "betreff":
                    _insertField = Field.Subject;
                    break;
                case "nachricht":
                    _insertField = Field.Body;
                    break;
                case "sende":
                    SendMail();
                    break;
            }
        }

        private void SendMail()
        {
            MailMessage mail = new MailMessage("kilianeller12@outlook.com", Mail.To);
            NetworkCredential networkCredential = new NetworkCredential("","");

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp-mail.outlook.com";
            client.Credentials = networkCredential;
            mail.Subject = Mail.Subject;
            mail.Body = Mail.Body;
            client.EnableSsl = true;

            client.Send(mail);
        }
        
    }
}