using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using MyPortfolio.Models;
using System.Web.Configuration;

namespace BlogProject.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Thanks()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var emailFrom = WebConfigurationManager.AppSettings["username"];

                    MailMessage mM = new MailMessage();
                    mM.From = new MailAddress(emailFrom);
                    mM.To.Add(WebConfigurationManager.AppSettings["emailto"]);
                    mM.Subject = model.name + " has sent you an email from your blog";
                    mM.Body = model.message + " Email: " + model.email;
                    mM.IsBodyHtml = true;

                    SmtpClient SmtpServer = new SmtpClient();

                    SmtpServer.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["username"], WebConfigurationManager.AppSettings["password"]);
                    SmtpServer.Port = int.Parse(WebConfigurationManager.AppSettings["port"]);
                    SmtpServer.Host = WebConfigurationManager.AppSettings["host"];
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mM);

                    return RedirectToAction("Thanks");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This shouldn't happen");
                }
            }

            return View("Contact", model);
        }
    }
}