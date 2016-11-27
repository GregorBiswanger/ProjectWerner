using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using S22.Imap;
using UserControl = System.Windows.Controls.UserControl;


namespace ProjectWerner.EmailClient.Views
{


    /// <summary>
    /// Interaction logic for ReadMailView.xaml
    /// </summary>
    public partial class ReadMailView : UserControl
    {
        public List<string> EmailList { get; set; }

        public ReadMailView()
        {
            InitializeComponent();
            Start();
        }

        public void Start()
        {
            EmailList = new List<string>();

            ImapClient client = new ImapClient("imap-mail.outlook.com", 993, true);
                
                client.Login("kilianeller12@outlook.com", "Cs870g797863",AuthMethod.Auto);
            IEnumerable<uint> uids = client.Search(SearchCondition.New());
            
            
                IEnumerable<MailMessage> messages = client.GetMessages(uids);

            foreach (var VARIABLE in messages)
            {
                EmailList.Add(VARIABLE.Subject);
            }


        }

    }

}
