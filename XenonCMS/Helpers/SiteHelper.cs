using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace XenonCMS.Helpers
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
    }
}