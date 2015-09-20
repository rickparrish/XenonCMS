using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
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
            return (T)MemoryCache.Default.Get(key);
        }

        protected static void RemoveCache(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        protected static void RemoveCacheMulti(string keyPrefix)
        {
            foreach (KeyValuePair<string, object> KVP in MemoryCache.Default.Where(x => x.Key.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                RemoveCache(KVP.Key);
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
                MemoryCache.Default.Add(key, value, DateTime.Now.AddMinutes(5));
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
                MemoryCache.Default.Add(key, value, new CacheItemPolicy()
                {
                    SlidingExpiration = new TimeSpan(0, minutes, 0)
                });
            }
        }
        #endregion


        public static SiteBlogPost GetBlogPost(int id)
        {
            return GetBlogPosts().SingleOrDefault(x => x.Id == id);
        }

        public static List<SiteBlogPost> GetBlogPosts()
        {
            string RequestDomain = Globals.GetRequestDomain();
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

        public static string GetNavBarStyle()
        {
            return (GetSite()?.NavBarInverted ?? false) ? "inverse" : "default";
        }

        public static string GetNavMenu(bool rightAlign, UrlHelper urlHelper)
        {
            // Get current page
            string CurrentUrl = HttpContext.Current.Request.RawUrl.Trim('/').ToLower();
            if (CurrentUrl.EndsWith("/index")) CurrentUrl = CurrentUrl.Substring(0, CurrentUrl.LastIndexOf("/"));
            if (string.IsNullOrWhiteSpace(CurrentUrl)) CurrentUrl = "home";

            // Get database entries TODO IsInRole("SiteAdmin") isn't great because it means a siteadmin for one domain can login and be a siteadmin for another domain
            List<NavMenuItem> NavMenuItems = Caching.GetNavMenuItems(HttpContext.Current.User.IsInRole("GlobalAdmin") || HttpContext.Current.User.IsInRole("SiteAdmin"), rightAlign);

            StringBuilder Result = new StringBuilder();

            foreach (var NMI in NavMenuItems)
            {
                // Determine if we have children
                if (NMI.Children == null)
                {
                    string Class = (NMI.Url.ToLower() == CurrentUrl) ? " class=\"active\"" : "";
                    Result.AppendLine("<li" + Class + "><a href=\"" + urlHelper.Content("~/" + (NMI.Url == "home" ? "" : NMI.Url)) + "\">" + NMI.Text + "</a></li>");
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

        public static List<NavMenuItem> GetNavMenuItems(bool isAdmin, bool rightAlign)
        {
            string RequestDomain = Globals.GetRequestDomain();
            string CacheKey = $"{CacheKeys.NavMenuItems.ToString()}-{RequestDomain}-{isAdmin}-{rightAlign}";

            List<NavMenuItem> Result = GetCache<List<NavMenuItem>>(CacheKey);
            if (Result == null)
            {
                Result = new List<NavMenuItem>();
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    // Get nav menu
                    var TopLevelPages = GetPages().Where(x => x.ShowInMenu && (x.ParentId == null) && (isAdmin || !x.RequireAdmin) && (x.RightAlign == rightAlign)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.LinkText).ToArray();
                    foreach (var TopLevelPage in TopLevelPages)
                    {
                        // Build the menu item
                        NavMenuItem NewMenuItem = new NavMenuItem(TopLevelPage.Id, TopLevelPage.LinkText, TopLevelPage.Slug);

                        // Determine if we have children
                        if (TopLevelPage.Children.Count > 0)
                        {
                            NewMenuItem.Children = new List<NavMenuItem>();
                            foreach (var ChildPage in TopLevelPage.Children)
                            {
                                NewMenuItem.Children.Add(new NavMenuItem(ChildPage.Id, ChildPage.LinkText, ChildPage.Slug));
                            }
                        }

                        Result.Add(NewMenuItem);
                    }
                }

                SetCacheSliding(CacheKey, Result);
            }

            return Result;
        }

        public static SitePage GetPage(string slug)
        {
            // Clean up the slug parameter
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "home";
            }
            else
            {
                slug = slug.ToLower().Trim('/');
                if (slug.EndsWith("/index")) slug = slug.Substring(0, slug.LastIndexOf("/"));
                if (string.IsNullOrWhiteSpace(slug))
                {
                    slug = "home";
                }
            }

            return GetPages().SingleOrDefault(x => x.Slug == slug);
        }

        public static List<SitePage> GetPages()
        {
            string RequestDomain = Globals.GetRequestDomain();
            string CacheKey = $"{CacheKeys.Pages.ToString()}-{RequestDomain}";

            List<SitePage> Result = GetCache<List<SitePage>>(CacheKey);
            if (Result == null)
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.SitePages.Include("Children").Include("Parent").Where(x => x.Site.Domain == RequestDomain).ToList();
                    if (Result != null)
                    {
                        SetCacheSliding(CacheKey, Result);
                    }
                }
            }

            return Result;
        }

        public static string GetSidebar()
        {
            string Result = ""; // TODOXXX DatabaseCache.GetSidebars();
            if (Result == null)
            {
                string RequestDomain = Globals.GetRequestDomain();

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
                // TODOXXX DatabaseCache.AddSidebars(Result);
            }

            return Result;
        }

        public static Site GetSite()
        {
            string RequestDomain = Globals.GetRequestDomain();
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

        public static string GetTheme()
        {
            return GetSite()?.Theme ?? "Cerulean";
        }

        public static long GetTimestamp(string path)
        {
            string RequestDomain = Globals.GetRequestDomain();
            string CacheKey = $"{CacheKeys.Timestamp.ToString()}-{path}";

            long? Result = GetCache<long?>(CacheKey);
            if (Result == null)
            {
                Result = File.GetLastWriteTimeUtc(path).ToFileTime();
                SetCacheAbsolute(CacheKey, Result);
            }

            return Convert.ToInt64(Result);
        }

        public static string GetSiteTitle()
        {
            return GetSite()?.Title ?? Globals.GetRequestDomain();
        }

        public static void ResetBlogPosts()
        {
            string RequestDomain = Globals.GetRequestDomain();
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
        }

        public static void ResetPages()
        {
            string RequestDomain = Globals.GetRequestDomain();
            RemoveCache($"{CacheKeys.Pages.ToString()}-{RequestDomain}");
            RemoveCacheMulti($"{CacheKeys.NavMenuItems.ToString()}-{RequestDomain}-");
        }

        public static void ResetSite()
        {
            string RequestDomain = Globals.GetRequestDomain();
            string CacheKey = $"{CacheKeys.Site.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
        }

        private enum CacheKeys
        {
            BlogPosts,
            NavMenuItems,
            Pages,
            Site,
            Timestamp
        }
    }
}