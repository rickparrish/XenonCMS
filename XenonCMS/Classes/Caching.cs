using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using XenonCMS.Models;
using XenonCMS.ViewModels.Blog;

namespace XenonCMS.Classes
{
    public class Caching
    {
        public static SiteBlogPost GetBlogPost(int id, HttpContextBase httpContext)
        {
            return GetBlogPosts(httpContext).SingleOrDefault(x => x.Id == id);
        }

        public static List<SiteBlogPost> GetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";

            List<SiteBlogPost> Result = null;
            if (httpContext.Cache[CacheKey] != null)
            {
                Result = (List<SiteBlogPost>)httpContext.Cache[CacheKey];
            }
            else
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.SiteBlogPosts.Where(x => x.Site.Domain == RequestDomain).ToList();
                    httpContext.Cache.Add(CacheKey, Result, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
                }
            }

            return Result;
        }

        public static Site GetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Site.ToString()}-{RequestDomain}";

            Site Result = null;
            if (httpContext.Cache[CacheKey] != null)
            {
                Result = (Site)httpContext.Cache[CacheKey];
            }
            else
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                    httpContext.Cache.Add(CacheKey, Result, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
                }
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

            List<SitePage> Result = null;
            if (httpContext.Cache[CacheKey] != null)
            {
                Result = (List<SitePage>)httpContext.Cache[CacheKey];
            }
            else
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Result = DB.SitePages.Where(x => x.Site.Domain == RequestDomain).ToList();
                    httpContext.Cache.Add(CacheKey, Result, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
                }
            }

            return Result;
        }

        public static void ResetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";
            httpContext.Cache.Remove(CacheKey);
        }

        public static void ResetPages(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Pages.ToString()}-{RequestDomain}";
            httpContext.Cache.Remove(CacheKey);
        }

        public static void ResetSite(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Site.ToString()}-{RequestDomain}";
            httpContext.Cache.Remove(CacheKey);
        }

        private enum CacheKeys
        {
            BlogPosts,
            Pages,
            Site
        }
    }
}