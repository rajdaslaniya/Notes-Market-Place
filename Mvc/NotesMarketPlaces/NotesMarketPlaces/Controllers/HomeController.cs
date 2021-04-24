using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using NotesMarketPlaces.Send_Mail;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();
        [Route("Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("FAQ")]
        public ActionResult FAQ()
        {
            ViewBag.FAQ = "active";
            return View();
        }
        
        // Get:Home/ContactUs
        [HttpGet]
        [Route("ContactUs")]
        public ActionResult ContactUs()
        {
            ViewBag.ContactUs = "active";
            if (User.Identity.IsAuthenticated)
            {
                var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                ContactUsViewModel contact = new ContactUsViewModel();
                contact.FullName = user.FirstName +" "+ user.LastName;
                return View(contact);
            }
            else
            {
                return View();
            }
        }
        // Post:Home/ContactUs
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ContactUs")]
        public ActionResult ContactUs(ContactUsViewModel contactusviewmodel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ContactUs = "active";
                BuildEmailContactTemplate(contactusviewmodel);
                ModelState.Clear();
                return View();
            }
            else
            {
                return View(contactusviewmodel);
            }
        }
        public void BuildEmailContactTemplate(ContactUsViewModel contactusviewmodel)
        {
            //Get All Text From Contact Us Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "ContactUs" + ".cshtml");
            //Replace Comments and Full Name
            body = body.Replace("@ViewBag.Comments", contactusviewmodel.Comments);
            body = body.Replace("@ViewBag.Name", contactusviewmodel.FullName);
            body = body.ToString();

            //Get Support Email
            var fromemail = _dbContext.SystemConfigurations.Where(x=>x.Key== "SupportEmail").FirstOrDefault();
            var tomail = _dbContext.Users.Where(x=>x.EmailID==contactusviewmodel.EmailID).FirstOrDefault();
            //From ,to,subject
            string from, to, subject;
            subject = contactusviewmodel.Subject+"-Query";
            from = fromemail.Value.Trim();
            to = tomail.EmailID.Trim();
            //Creating a mail
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from, "NotesMarketPlaces");
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendingMail.SendEmail(mail);
         }
    }
}