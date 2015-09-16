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
        #region Helper Methods
        private static T GetCache<T>(string key)
        {
            return (T)HttpContext.Current.Cache[key];
        }

        protected static void RemoveCache(string key)
        {
            HttpContext.Current.Cache.Remove(key);
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

        public static void ResetBlogPosts(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.BlogPosts.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
        }

        public static void ResetPages(HttpContextBase httpContext)
        {
            string RequestDomain = Globals.GetRequestDomain(httpContext);
            string CacheKey = $"{CacheKeys.Pages.ToString()}-{RequestDomain}";
            RemoveCache(CacheKey);
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
            Pages,
            Site
        }
    }
}