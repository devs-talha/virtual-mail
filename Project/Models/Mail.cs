using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Mail
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }

        public Mail()
        {
        }

        public Mail(int id, User sender, User receiver, string subject, string body, DateTime date)
        : this(sender, receiver, subject, body, date)
        {
            Id = id;
        }

        public Mail(User sender, User receiver, string subject, string body, DateTime date)
        {
            Sender = sender;
            Receiver = receiver;
            Subject = subject;
            Body = body;
            Date = date;
        }
    }
}
