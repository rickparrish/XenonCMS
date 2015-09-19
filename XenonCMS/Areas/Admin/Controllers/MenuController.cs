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
                // Determine if we have children
                if (NMI.Children == null)
                {
                    Result.AppendLine("<li data-id=\"" + NMI.Id.ToString() + "\" data-slug=\"" + NMI.Url + "\" class=\"dropdown\"><a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + NMI.Text + " <b class=\"caret\"></b></a>");
                    Result.AppendLine("<ul class=\"dropdown-menu\">");
                    Result.AppendLine("</ul>");
                    Result.AppendLine("</li>");
                }
                else
                {
                    Result.AppendLine("<li class=\"dropdown\"><a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + NMI.Text + " <b class=\"caret\"></b></a>");
                    Result.AppendLine("<ul class=\"dropdown-menu\">");

                    foreach (var CNMI in NMI.Children)
                    {
                        Result.AppendLine("<li data-id=\"" + NMI.Id.ToString() + "\" data-slug=\"" + NMI.Url + "\"><a href=\"#\">" + CNMI.Text + "</a></li>");
                    }

                    Result.AppendLine("</ul>");
                    Result.AppendLine("</li>");
                }
            }

            ViewBag.Menu = Result.ToString();

            return View();
        }
    }
}
