using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XenonCMS.Models;

namespace XenonCMS.Classes
{
    static public class LayoutHelper
    {
        public static bool LeftAligned = false;
        public static bool RightAligned = true;

        private static List<NavMenuItem> GetNavMenuItems(HttpContextBase httpContext, bool isAdmin, bool rightAlign)
        {
            List<NavMenuItem> Result = DatabaseCache.GetNavMenuItems(httpContext, isAdmin, rightAlign);
            if (Result == null)
            {
                Result = new List<NavMenuItem>();
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get nav menu
                    var NavMenuItems = DB.SitePages.Where(x => (x.Site.Domain == RequestDomain) && x.ShowInMenu && (x.ParentId == 0) && (isAdmin || !x.RequireAdmin) && (x.RightAlign == rightAlign)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Text).ToArray();
                    foreach (var NMI in NavMenuItems)
                    {
                        // Build the menu item
                        NavMenuItem NewMenuItem = new NavMenuItem(NMI.Text, NMI.Slug);

                        // Determine if we have children
                        var Children = DB.SitePages.Where(x => x.ShowInMenu && (x.ParentId == NMI.Id) && (isAdmin || !x.RequireAdmin) && (x.RightAlign == rightAlign)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Text);
                        if (Children.Count() > 0)
                        {
                            NewMenuItem.Children = new List<NavMenuItem>();
                            foreach (var C in Children)
                            {
                                NewMenuItem.Children.Add(new NavMenuItem(C.Text, C.Slug));
                            }
                        }

                        Result.Add(NewMenuItem);
                    }
                }

                DatabaseCache.AddNavMenuItems(httpContext, Result, isAdmin, rightAlign);
            }

            return Result;
        }

        public static string NavBarStyle(HttpContextBase httpContext)
        {
            Site Site = DatabaseCache.GetSite(httpContext);
            if (Site == null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get site title
                    Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                }

                if (Site == null)
                {
                    return "default";
                }
                else
                {
                    DatabaseCache.AddSite(httpContext, Site);
                }
            }

            return (Site.NavBarInverted ? "inverse" : "default");
        }

        public static string NavMenu(bool rightAlign, HttpContextBase httpContext, UrlHelper urlHelper)
        {
            // Get current page
            string CurrentUrl = httpContext.Request.RawUrl.Trim('/').ToLower();
            if (CurrentUrl.EndsWith("/index")) CurrentUrl = CurrentUrl.Substring(0, CurrentUrl.LastIndexOf("/"));
            if (string.IsNullOrWhiteSpace(CurrentUrl)) CurrentUrl = "home";

            // Get database entries
            List<NavMenuItem> NavMenuItems = GetNavMenuItems(httpContext, httpContext.User.IsInRole("GlobalAdmin") || httpContext.User.IsInRole("SiteAdmin"), rightAlign);

            StringBuilder Result = new StringBuilder();

            foreach (var NMI in NavMenuItems)
            {
                // Determine if we have children
                if (NMI.Children == null)
                {
                    string Class = (NMI.Url.ToLower() == CurrentUrl) ? " class=\"active\"" : "";
                    Result.AppendLine("<li" + Class + "><a href=\"" + urlHelper.Content("~/" + NMI.Url) + "\">" + NMI.Text + "</a></li>");
                }
                else
                {
                    string ParentClass = (NMI.Url.ToLower() == CurrentUrl) ? " class=\"active dropdown\"" : " class=\"dropdown\"";
                    Result.AppendLine("<li" + ParentClass + "><a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + NMI.Text + " <b class=\"caret\"></b></a>");
                    Result.AppendLine("<ul class=\"dropdown-menu\">");

                    foreach (var CNMI in NMI.Children)
                    {
                        string ChildClass = (CNMI.Url.ToLower() == CurrentUrl) ? " class=\"active\"" : "";
                        Result.AppendLine("<li" + ChildClass + "><a href=\"" + urlHelper.Content("~/" + CNMI.Url) + "\">" + CNMI.Text + "</a></li>");
                    }

                    Result.AppendLine("</ul>");
                    Result.AppendLine("</li>");
                }
            }

            return Result.ToString();
        }

        public static string Sidebar(HttpContextBase httpContext)
        {
            string Result = DatabaseCache.GetSidebars(httpContext);
            if (Result == null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get site side bar
                    var Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                    if (Site != null) Result += Site.Sidebar;

                    // Get global side bar(s)
                    var GlobalSidebars = DB.SiteGlobalSidebars.Where(x => x.Site.Domain == RequestDomain).OrderBy(x => x.GlobalSidebar.DisplayOrder);
                    foreach (var Sidebar in GlobalSidebars)
                    {
                        Result += Sidebar.GlobalSidebar.Html;
                    }
                }

                if (Result == null) Result = "";
                DatabaseCache.AddSidebars(httpContext, Result);
            }

            return Result;
        }

        public static string SiteTitle(HttpContextBase httpContext)
        {
            Site Site = DatabaseCache.GetSite(httpContext);
            if (Site == null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get site title
                    Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                }

                if (Site == null)
                {
                    return Globals.GetRequestDomain(httpContext);
                }
                else
                {
                    DatabaseCache.AddSite(httpContext, Site);
                }
            }
            
            return Site.Title;
        }

        public static string Theme(HttpContextBase httpContext)
        {
            Site Site = DatabaseCache.GetSite(httpContext);
            if (Site == null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get site title
                    Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                }

                if (Site == null)
                {
                    return "Cerulean";
                }
                else
                {
                    DatabaseCache.AddSite(httpContext, Site);
                }
            }

            return Site.Theme;
        }

        public class NavMenuItem
        {
            public List<NavMenuItem> Children;
            public string Text;
            public string Url;

            public NavMenuItem(string text, string url)
            {
                Text = text;
                Url = url;
            }
        }
    }
}