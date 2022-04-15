using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Utils
{
    public class Email
    {
        private readonly MimeMessage mimeMessage;
        private readonly string _senderName = "Justice Kutch";
        private readonly string _senderEmail = "justice.kutch78@ethereal.email";
        private readonly string _senderPassword = "6X2pgyhHz6KDnx9m5J";
        private readonly string _host = "smtp.ethereal.email";
        private readonly int _port = 587;
        private readonly SecureSocketOptions _options = SecureSocketOptions.StartTls;

        public Email(string receiverEmail)
        {
            mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_senderName, _senderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(receiverEmail));
        }

        public void Create(string subject, string body)
        {
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(TextFormat.Plain)
            {
                Text = body
            };
        }

        public void Send()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Connect(_host, _port, _options);
            smtp.Authenticate(_senderEmail, _senderPassword);
            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
        }
    }
}
