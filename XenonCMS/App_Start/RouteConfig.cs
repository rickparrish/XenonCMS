using System.Web.Mvc;
using System.Web.Routing;
using XenonCMS.Helpers;

namespace XenonCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CmsInstall",
                url: "Cms/Install",
                defaults: new { controller = "Cms", action = "Install" }
            );

            // http://stackoverflow.com/a/16030904
            routes.MapRoute(
                name: "CmsIndex",
                url: "{*url}",
                defaults: new { controller = "Cms", action = "Index" },
                constraints: new { url = new CmsUrlConstraint() }
            );

            routes.MapRoute(
                name: "BlogPost",
                url: "Blog/{id}/{year}/{month}/{day}/{slug}",
                defaults: new { controller = "Blog", action = "Details", year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional, slug = UrlParameter.Optional },
                constraints: new { controller = "Blog", id = @"\d*" }
            );

            routes.MapRoute(
                name: "Image",
                url: "Images/{id}",
                defaults: new { controller = "Images", action = "Image" },
                constraints: new { controller = "Images" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
