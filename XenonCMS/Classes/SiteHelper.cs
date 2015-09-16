using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using XenonCMS.Models;

namespace XenonCMS.Classes
{
    public class SiteHelper
    {
        public static string AbsoluteFilename(HttpContextBase httpContext, string filename)
        {
            return AbsoluteFilename(httpContext, filename, "");
        }

        public static string AbsoluteFilename(HttpContextBase httpContext, string filename, string directory)
        {
            string SiteFilesDirectory = HostingEnvironment.MapPath("~/SiteFiles/" + Globals.GetRequestDomain(httpContext) + "/" + directory);
            string RequestedFile = Path.Combine(SiteFilesDirectory, filename.Trim('/').Replace('/', '\\'));

            if (RequestedFile.ToLower().StartsWith(SiteFilesDirectory.ToLower()))
            {
                return RequestedFile;
            }
            else
            {
                throw new HttpException(404, "Not found");
            }
        }

        public static bool FileExists(HttpContextBase httpContext, string filename)
        {
            string SiteFilesDirectory = HostingEnvironment.MapPath("~/SiteFiles/" + Globals.GetRequestDomain(httpContext));
            string RequestedFile = Path.Combine(SiteFilesDirectory, filename.Trim('/').Replace('/', '\\'));
            return (RequestedFile.ToLower().StartsWith(SiteFilesDirectory.ToLower()) && File.Exists(RequestedFile));
        }

        public static bool SiteExists(HttpContextBase httpContext)
        {
            return (Caching.GetSite(httpContext) != null);
        }

        public static bool SlugExists(HttpContextBase httpContext, string slug)
        {
            // Clean up the url parameter (TODO Duplicated in Cms/Slug)
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

            // TODO Should have caching for this
            using (var DB = new ApplicationDbContext())
            {
                return DB.SitePages.Where(x => x.Slug.ToLower() == slug.ToLower()).Any();
            }
        }
    }
}