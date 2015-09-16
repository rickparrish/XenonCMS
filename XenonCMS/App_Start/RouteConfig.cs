using System.Web.Mvc;
using System.Web.Routing;
using XenonCMS.Classes;

namespace XenonCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = "Account|Blog|Cms|Contact|Errors" } /*TODO Can this be done dynamically? Ie pull a list of valid controllers/actions?*/
            );

            routes.MapRoute(
                name: "BlogPost",
                url: "Blog/{id}/{year}/{month}/{day}/{slug}",
                defaults: new { controller = "Blog", action = "Details", year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional, slug = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "NewsPost",
                url: "News/{id}/{year}/{month}/{day}/{slug}",
                defaults: new { controller = "Blog", action = "Details", year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional, slug = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "CmsSlug",
                url: "{*slug}",
                defaults: new { controller = "Cms", action = "Slug" }
            );
        }
    }
}
