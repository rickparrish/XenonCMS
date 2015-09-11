using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using XenonCMS.Models;

namespace XenonCMS.Classes
{
    public class DatabaseCache
    {
        static public void AddAdminIPs(HttpContextBase httpContext, List<string> adminIPs, bool globalOnly)
        {
            if (adminIPs != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = globalOnly ? "GlobalAdminIPs" : "AdminIPs-" + RequestDomain;
                httpContext.Cache.Add(CacheKey, adminIPs, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddBlogPost(HttpContextBase httpContext, SiteBlogPost blogPost)
        {
            if (blogPost != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "BlogPost-" + RequestDomain + "-" + blogPost.Id.ToString();
                httpContext.Cache.Add(CacheKey, blogPost, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddBlogPosts(HttpContextBase httpContext, List<SiteBlogPost> blogPosts)
        {
            if (blogPosts != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "BlogPosts-" + RequestDomain;
                httpContext.Cache.Add(CacheKey, blogPosts, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddNavMenuItems(HttpContextBase httpContext, List<LayoutHelper.NavMenuItem> navMenuItems, bool isAdmin, bool rightAlign)
        {
            if (navMenuItems != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "NavMenuItems-" + RequestDomain + "-" + isAdmin.ToString() + "-" + rightAlign.ToString();
                httpContext.Cache.Add(CacheKey, navMenuItems, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddSite(HttpContextBase httpContext, Site site)
        {
            if (site != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "Site-" + RequestDomain;
                httpContext.Cache.Add(CacheKey, site, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddSitePage(HttpContextBase httpContext, SitePage page)
        {
            if (page != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "SitePage-" + RequestDomain + "-" + page.Slug.ToLower();
                httpContext.Cache.Add(CacheKey, page, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public void AddSidebars(HttpContextBase httpContext, string sidebars)
        {
            if (sidebars != null)
            {
                string RequestDomain = Globals.GetRequestDomain(httpContext);
                string CacheKey = "Sidebars-" + RequestDomain;
                httpContext.Cache.Add(CacheKey, sidebars, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
        }

        static public List<string> GetAdminIPs(HttpContextBase httpContext, bool globalOnly)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = globalOnly ? "GlobalAdminIPs" : "AdminIPs-" + RequestDomain;
            return (List<string>)httpContext.Cache[CacheKey];
        }

        static public SiteBlogPost GetBlogPost(HttpContextBase httpContext, int id)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "BlogPost-" + RequestDomain + "-" + id.ToString();
            return (SiteBlogPost)httpContext.Cache[CacheKey];
        }

        static public List<SiteBlogPost> GetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "BlogPosts-" + RequestDomain;
            return (List<SiteBlogPost>)httpContext.Cache[CacheKey];
        }

        static public List<LayoutHelper.NavMenuItem> GetNavMenuItems(HttpContextBase httpContext, bool isAdmin, bool rightAlign)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "NavMenuItems-" + RequestDomain + "-" + isAdmin.ToString() + "-" + rightAlign.ToString();
            return (List<LayoutHelper.NavMenuItem>)httpContext.Cache[CacheKey];
        }

        internal static string GetSidebars(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "Sidebars-" + RequestDomain;
            return (string)httpContext.Cache[CacheKey];
        }

        static public Site GetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "Site-" + RequestDomain;
            return (Site)httpContext.Cache[CacheKey];
        }

        static public SitePage GetSitePage(HttpContextBase httpContext, string url)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "SitePage-" + RequestDomain + "-" + url.ToLower();
            return (SitePage)httpContext.Cache[CacheKey];
        }

        static public void RemoveBlogPost(HttpContextBase httpContext, int id)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "BlogPost-" + RequestDomain + id.ToString();
            httpContext.Cache.Remove(CacheKey);
        }

        static public void RemoveSitePage(HttpContextBase httpContext, string url)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "SitePage-" + RequestDomain + "-" + url.ToLower();
            httpContext.Cache.Remove(CacheKey);
        }

        static public void ResetAdminIPs(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "AdminIPs-" + RequestDomain;
            httpContext.Cache.Remove(CacheKey);
        }

        static public void ResetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "BlogPosts-" + RequestDomain;
            httpContext.Cache.Remove(CacheKey);
        }

        static public void ResetNavMenuItems(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "NavMenuItems-" + RequestDomain + "-" + true.ToString() + "-" + true.ToString();
            httpContext.Cache.Remove(CacheKey);
            CacheKey = "NavMenuItems-" + RequestDomain + "-" + true.ToString() + "-" + false.ToString();
            httpContext.Cache.Remove(CacheKey);
            CacheKey = "NavMenuItems-" + RequestDomain + "-" + false.ToString() + "-" + true.ToString();
            httpContext.Cache.Remove(CacheKey);
            CacheKey = "NavMenuItems-" + RequestDomain + "-" + false.ToString() + "-" + false.ToString();
            httpContext.Cache.Remove(CacheKey);
        }

        static public void ResetSidebars(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "Sidebars-" + RequestDomain;
            httpContext.Cache.Remove(CacheKey);
        }

        static public void ResetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = "Site-" + RequestDomain;
            httpContext.Cache.Remove(CacheKey);
        }

        #region BlogDetails
        internal static string _BlogDetailsKey = "BlogDetails-";
        internal static void AddBlogDetails(ViewModels.Blog.Details blogPost, HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogDetailsKey + RequestDomain + "-" + blogPost.Id.ToString();
            httpContext.Cache.Add(CacheKey, blogPost, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
        }

        internal static ViewModels.Blog.Details GetBlogDetails(int id, ApplicationDbContext db, HttpContextBase httpContext)
        {
            // Pull from cache
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogDetailsKey + RequestDomain + "-" + id.ToString();
            var Result = (ViewModels.Blog.Details)httpContext.Cache[CacheKey];

            if (Result == null)
            {
                // Pull from database
                var BlogPost = db.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
                if (BlogPost != null)
                {
                    // Database model to view model
                    Result = ModelConverter.Convert<ViewModels.Blog.Details>(BlogPost);

                    // Add to cache
                    DatabaseCache.AddBlogDetails(Result, httpContext);
                }
            }

            return Result;
        }

        internal static void RemoveBlogDetails(int id, HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogDetailsKey + RequestDomain + "-" + id.ToString();
            httpContext.Cache.Remove(CacheKey);
        }
        #endregion

        #region BlogIndex
        internal static string _BlogIndexKey = "BlogIndex-";
        private static void AddBlogIndex(List<ViewModels.Blog.Index> blogPosts, HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogIndexKey + RequestDomain;
            httpContext.Cache.Add(CacheKey, blogPosts, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
        }

        internal static List<ViewModels.Blog.Index> GetBlogIndex(ApplicationDbContext db, HttpContextBase httpContext)
        {
            // Pull from cache
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogIndexKey + RequestDomain;
            var Result = (List<ViewModels.Blog.Index>)httpContext.Cache[CacheKey];

            if (Result == null)
            {
                // Pull from database
                var BlogPosts = db.SiteBlogPosts.Where(x => x.Site.Domain == RequestDomain).OrderByDescending(x => x.DatePosted).ToArray();
                if (BlogPosts != null)
                {
                    // Database model to view model
                    Result = ModelConverter.Convert<ViewModels.Blog.Index>(BlogPosts);

                    // Add to cache
                    DatabaseCache.AddBlogIndex(Result, httpContext);
                }
            }

            return Result;
        }

        internal static void ResetBlogIndex(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = _BlogIndexKey + RequestDomain;
            httpContext.Cache.Remove(CacheKey);
        }
        #endregion
    }
}