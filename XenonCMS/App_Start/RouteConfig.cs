using System.Linq;
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

            // Default route that is limited to just the XenonCMS.Controllers controllers
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = GetAllControllersAsRegex("XenonCMS.Controllers") }
            );

            // Admin route that is limited to just the XenonCMS.Areas.Admin.Controllers controllers
            routes.MapRoute(
                name: "AdminArea",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { area="Admin", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = GetAllControllersAsRegex("XenonCMS.Areas.Admin.Controllers") }
            );

            // Handle blog post requests
            routes.MapRoute(
                name: "BlogPost",
                url: "Blog/{id}/{year}/{month}/{day}/{slug}",
                defaults: new { controller = "Blog", action = "Details", year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional, slug = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            // Catch all for anything that wasn't matched above
            routes.MapRoute(
                name: "CmsSlug",
                url: "{*slug}",
                defaults: new { controller = "Cms", action = "Slug" }
            );
        }

        // From: http://stackoverflow.com/a/4668252/342378 (comment)
        private static string GetAllControllersAsRegex(string namespaceConstraint) {
            var controllers = typeof(MvcApplication).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Controller)) && (t.Namespace == namespaceConstraint));
            var controllerNames = controllers.Select(c => c.Name.Replace("Controller", ""));
            return string.Format("({0})", string.Join("|", controllerNames));
        }
    }
}
