using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using XenonCMS.Areas.Admin.ViewModels.Pages;
using XenonCMS.Classes;
using XenonCMS.Models;

namespace XenonCMS.Areas.Admin.Controllers
{
    [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
    public class MenuController : Controller
    {
        // GET: /Admin/Pages/
        public ActionResult Index()
        {
            // TODO Include hidden items (flag them as such)
            // TODO Flag admin items as such

            // Get database entries
            // TODO This only handles left-aligned items.  Should we allow right-aligned, or keep that just for admin?
            List<NavMenuItem> NavMenuItems = Caching.GetNavMenuItems(true, false);

            StringBuilder Result = new StringBuilder();

            foreach (var NMI in NavMenuItems)
            {
                Result.AppendLine("<li data-id=\"" + NMI.Id.ToString() + "\" data-text=\"" + Server.HtmlEncode(NMI.Text) + "\">" + NMI.Text);
                Result.AppendLine("<ol>");
                if (NMI.Children != null)
                {
                    foreach (var CNMI in NMI.Children)
                    {
                        Result.AppendLine("<li data-id=\"" + CNMI.Id.ToString() + "\" data-text=\"" + Server.HtmlEncode(CNMI.Text) + "\">" + CNMI.Text + "<ol></ol></li>");
                    }
                }
                Result.AppendLine("</ol>");
                Result.AppendLine("</li>");
            }

            ViewBag.Menu = Result.ToString();

            return View();
        }
    }
}
