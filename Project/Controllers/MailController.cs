using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class MailController : Controller
    {
        private MailBLO mailBLO = new MailBLO();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Compose()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult Compose(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int referenceMail = int.Parse(formCollection["referenceMail"].ToString());
            string to = formCollection["to"].ToString();
            string subject = formCollection["subject"].ToString();
            string body = formCollection["body"].ToString();

            User sender = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));

            try
            {
                if (referenceMail == -1) { 
                    mailBLO.Send(to, subject, body, sender);
                } else
                {
                    mailBLO.SendWithReference(to, subject, body, sender, referenceMail);
                }
            }
            catch (Exception e)
            {
                TempData["to"] = to;
                TempData["subject"] = subject;
                TempData["body"] = body;
                TempData["error"] = e.Message;
                return View();
            }

            TempData["success"] = "Email sent successfully";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Inbox()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            List<Mail> receivedMails = mailBLO.ListReceivedMails(user);
            ViewData.Add("mails", receivedMails);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult InboxEmailView(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int mailId = int.Parse(formCollection["id"].ToString());
            List<Mail> mails = mailBLO.GetWithReferences(mailId);
            ViewData.Add("mails", mails);
            return View("InboxEmailView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult InboxEmailDelete(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int mailId = int.Parse(formCollection["id"].ToString());
            try { 
                mailBLO.Delete(mailId);
            } catch (Exception e)
            {
                Mail mail = mailBLO.Get(mailId);
                ViewData.Add("mail", mail);
                TempData["error"] = e.Message;
                return View("InboxEmailView");
            }
            return RedirectToAction("Inbox");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult InboxEmailReply(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int mailId = int.Parse(formCollection["referenceMail"].ToString());
            Mail mail = mailBLO.Get(mailId);
            TempData["referenceMail"] = mail;
            return View("Compose");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SentEmails()
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            List<Mail> sentMails = mailBLO.ListSentMails(user);
            ViewData.Add("mails", sentMails);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult SentEmailView(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int mailId = int.Parse(formCollection["id"].ToString());
            Mail mail = mailBLO.Get(mailId);
            ViewData.Add("mail", mail);
            return View("SentEmailView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult SentEmailDelete(IFormCollection formCollection)
        {
            if (!ValidateSession())
                return RedirectToAction("RedirectToLogin");

            int mailId = int.Parse(formCollection["id"].ToString());
            try
            {
                mailBLO.Delete(mailId);
            }
            catch (Exception e)
            {
                Mail mail = mailBLO.Get(mailId);
                ViewData.Add("mail", mail);
                TempData["error"] = e.Message;
                return View("SentEmailView");
            }
            return RedirectToAction("SentEmails");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [NonAction]
        private bool ValidateSession()
        {
            return HttpContext.Session.Keys.Contains("user");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult RedirectToLogin()
        {
            TempData["error"] = "You must be logged in";
            return RedirectToAction("Login", "User");
        }
    }
}
