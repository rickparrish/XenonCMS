using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XenonCMS.Classes;

namespace XenonCMS.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string ContentWithTimestamp(this UrlHelper url, string contentPath)
        {
            return url.Content(contentPath) + "?ts=" + Caching.GetTimestamp(HttpContext.Current.Server.MapPath(contentPath));
        }
    }
}