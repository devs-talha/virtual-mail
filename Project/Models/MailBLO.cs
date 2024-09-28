using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class MailBLO
    {
        private UserDAO userDAO = new UserDAO();
        private MailDAO mailDAO = new MailDAO();

        public Mail Get(int id)
        {
            List<Mail> mails = mailDAO.GetMail(id, false);
            if (mails == null || mails.Count == 0)
                throw new Exception("No mail found with specified email");
            return mails.ElementAt<Mail>(0);
        }

        public List<Mail> GetWithReferences(int id)
        {
            List<Mail> mails = mailDAO.GetMail(id, true);
            if (mails == null || mails.Count == 0)
                throw new Exception("No mail found with specified email");
            return mails;
        }

        public void Delete(int id)
        {
            if (!mailDAO.DeleteMail(id))
                throw new Exception("Could not delete mail");
        }


        public void Send(string to, string subject, string body, User sender)
        {
            User receiver = userDAO.GetUser(to);
            if (receiver == null)
                throw new Exception("No user found with specified email");
            Mail mail = new Mail(sender, receiver, subject, body, DateTime.Now);
            if (!mailDAO.InsertMail(mail, -1))
                throw new Exception("Email sending failed");
        }

        public void SendWithReference(string to, string subject, string body, User sender, int referenceMail)
        {
            User receiver = userDAO.GetUser(to);
            if (receiver == null)
                throw new Exception("No user found with specified email");
            Mail mail = new Mail(sender, receiver, subject, body, DateTime.Now);
            if (!mailDAO.InsertMail(mail, referenceMail))
                throw new Exception("Email sending failed");
        }


        public List<Mail> ListReceivedMails(User receiver)
        {
            return mailDAO.ListReceivedMails(receiver);
        }

        public List<Mail> ListSentMails(User sender)
        {
            return mailDAO.ListSentMails(sender);
        }
    }

}
