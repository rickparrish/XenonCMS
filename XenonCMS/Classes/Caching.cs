using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using XenonCMS.Models;
using XenonCMS.ViewModels.Blog;

namespace XenonCMS.Classes
{
    public class Caching
    {
        #region Helper Methods
        private static T GetCache<T>(string key)
        {
            return (T)HttpContext.Current.Cache[key];
        }

        protected static void RemoveCache(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        protected static void RemoveCacheMulti(string keyPrefix)
        {
            foreach (DictionaryEntry DE in HttpContext.Current.Cache)
            {
                if (DE.Key.ToString().StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    RemoveCache(DE.Key.ToString());
                }
            }
        }

        private static void SetCacheAbsolute(string key, object value)
        {
            SetCacheAbsolute(key, value, 30);
        }

        private static void SetCacheAbsolute(string key, object value, int minutes)
        {
            if (value != null)
            {
                HttpContext.Current.Cache.Add(key, value, null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
        }

        private static void SetCacheSliding(string key, object value)
        {
            SetCacheSliding(key, value, 30);
        }

        private static void SetCacheSliding(string key, object value, int minutes)
        {
            if (value != null)
            {
                HttpContext.Current.Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, minutes, 0), CacheItemPriority.Normal, null);
            }
        }
        #endregion


        public static SiteBlogPost GetBlogPost(int id, HttpContextBase httpContext)
        {
            return GetBlogPosts(httpContext).SingleOrDefault(x => x.Id == id);
        }

        public static List<SiteBlogPost> GetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";

            List<SiteBlogPost> Result = GetCache<List<SiteBlogPost>>(CacheKey);
            if (Result == null)
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.SiteBlogPosts.Where(x => x.Site.Domain == RequestDomain).ToList();
                    if (Result != null)
                    {
                        SetCacheSliding(CacheKey, Result);
                    }
                }
            }

            return Result;
        }

        public static string GetNavBarStyle(HttpContextBase httpContext)
        {
            return (GetSite(httpContext)?.NavBarInverted ?? false) ? "inverse" : "default";
        }

        public static string GetNavMenu(bool rightAlign, HttpContextBase httpContext, UrlHelper urlHelper)
        {
            // Get current page
            string CurrentUrl = httpContext.Request.RawUrl.Trim('/').ToLower();
            if (CurrentUrl.EndsWith("/index")) CurrentUrl = CurrentUrl.Substring(0, CurrentUrl.LastIndexOf("/"));
            if (string.IsNullOrWhiteSpace(CurrentUrl)) CurrentUrl = "home";

            // Get database entries TODO IsInRole("SiteAdmin") isn't great because it means a siteadmin for one domain can login and be a siteadmin for another domain
            List<NavMenuItem> NavMenuItems = Caching.GetNavMenuItems(httpContext, httpContext.User.IsInRole("GlobalAdmin") || httpContext.User.IsInRole("SiteAdmin"), rightAlign);

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

        private static List<NavMenuItem> GetNavMenuItems(HttpContextBase httpContext, bool isAdmin, bool rightAlign)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.NavMenuItems.ToString()}-{RequestDomain}-{isAdmin}-{rightAlign}";

            List<NavMenuItem> Result = GetCache<List<NavMenuItem>>(CacheKey);
            if (Result == null)
            {
                Result = new List<NavMenuItem>();
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get nav menu
                    var TopLevelPages = GetPages(httpContext).Where(x => x.ShowInMenu && (x.ParentId == 0) && (isAdmin || !x.RequireAdmin) && (x.RightAlign == rightAlign)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Text).ToArray();
                    foreach (var TopLevelPage in TopLevelPages)
                    {
                        // Build the menu item
                        NavMenuItem NewMenuItem = new NavMenuItem(TopLevelPage.Text, TopLevelPage.Slug);

                        // Determine if we have children
                        var ChildPages = GetPages(httpContext).Where(x => x.ShowInMenu && (x.ParentId == TopLevelPage.Id) && (isAdmin || !x.RequireAdmin) && (x.RightAlign == rightAlign)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Text);
                        if (ChildPages.Count() > 0)
                        {
                            NewMenuItem.Children = new List<NavMenuItem>();
                            foreach (var ChildPage in ChildPages)
                            {
                                NewMenuItem.Children.Add(new NavMenuItem(ChildPage.Text, ChildPage.Slug));
                            }
                        }

                        Result.Add(NewMenuItem);
                    }
                }

                SetCacheSliding(CacheKey, Result);
            }

            return Result;
        }

        public static SitePage GetPage(string slug, HttpContextBase httpContext)
        {
            return GetPages(httpContext).SingleOrDefault(x => x.Slug == slug);
        }

        public static List<SitePage> GetPages(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Pages.ToString()}-{RequestDomain}";

            List<SitePage> Result = GetCache< List<SitePage>>(CacheKey);
            if (Result == null)
             {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.SitePages.Where(x => x.Site.Domain == RequestDomain).ToList();
                    if (Result != null)
                    {
                        SetCacheSliding(CacheKey, Result);
                    }
                }
            }

            return Result;
        }

        public static string GetSidebar(HttpContextBase httpContext)
        {
            string Result = ""; // TODOXXX DatabaseCache.GetSidebars(httpContext);
            if (Result == null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get site side bar
                    var Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                    if (Site != null) Result += Site.Sidebar;

                    // Get global side bar(s)
                    foreach (var Sidebar in Site.GlobalSidebars)
                    {
                        Result += Sidebar.Html;
                    }
                }

                if (Result == null) Result = "";
                // TODOXXX DatabaseCache.AddSidebars(httpContext, Result);
            }

            return Result;
        }

        public static Site GetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Site.ToString()}-{RequestDomain}";

            Site Result = GetCache<Site>(CacheKey);
            if (Result == null)
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                    if (Result != null)
                    {
                        SetCacheSliding(CacheKey, Result);
                    }
                }
            }

            return Result;
        }

        public static string GetTheme(HttpContextBase httpContext)
        {
            return GetSite(httpContext)?.Theme ?? "Cerulean";
        }

        public static string GetSiteTitle(HttpContextBase httpContext)
        {
            return GetSite(httpContext)?.Title ?? Globals.GetRequestDomain(httpContext);
        }

        public static void ResetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
        }

        public static void ResetPages(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            RemoveCache($"{CacheKeys.Pages.ToString()}-{RequestDomain}");
            RemoveCacheMulti($"{CacheKeys.NavMenuItems.ToString()}-{RequestDomain}-");
        }

        public static void ResetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Site.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
        }

        private enum CacheKeys
        {
            BlogPosts,
            NavMenuItems,
            Pages,
            Site
        }
    }
}