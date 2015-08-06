using System;
using System.Globalization;
using System.Net.Mail;
using System.Web.Mvc;
using XenonCMS.Helpers;
using XenonCMS.ViewModels.Contact;

namespace XenonCMS.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Contact/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Index model)
        {
            if (ModelState.IsValid)
            {
                // TODO Allow specifying smtp host, port, username, password, ssl, etc
                using (SmtpClient Smtp = new SmtpClient("localhost"))
                {
                    using (MailMessage Email = new MailMessage())
                    {
                        try
                        {
                            // Check for a url, and flag as spam if found
                            if (!string.IsNullOrWhiteSpace(model.Url)) model.Subject = "(CONTACT_SPAM) " + model.Subject;

                            // Send email
                            Email.From = new MailAddress(model.Email, model.Name);
                            Email.To.Add(DatabaseCache.GetSite(ControllerContext.RequestContext.HttpContext).ContactEmail);
                            Email.Subject = model.Subject;
                            Email.Body = model.Body;
                            Email.IsBodyHtml = false;
                            Smtp.Send(Email); 
                            
                            ViewBag.Message = "<strong>Success!</strong> Your message has been sent!";
                            ViewBag.MessageClass = "success";

                            ModelState["Subject"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
                            ModelState["Body"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            ViewBag.Message = "<strong>Error!</strong> Your message could not be sent, try again later!";
                            ViewBag.MessageClass = "danger";
                            // TODO Log exception
                        }
                    }
                }
               
                return View(model);
            }
            else
            {
                return View(model);
            }
        }
    }
}