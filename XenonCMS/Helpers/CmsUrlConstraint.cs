using System.Web;
using System.Web.Routing;

namespace XenonCMS.Helpers
{
    // http://stackoverflow.com/a/16030904
    public class CmsUrlConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // The CMS will handle url-less requests as requests for /Home
            if (values[parameterName] == null) return true;

            // Check if this is an admin or blog or contact request (which have built-in controllers)
            var url = values[parameterName].ToString().ToLower().Trim('/');
            if ((url == "admin") || (url.StartsWith("admin/"))) return false;
            if ((url == "blog") || (url.StartsWith("blog/"))) return false;
            if ((url == "contact") || (url.StartsWith("contact/"))) return false;
            if ((url == "images") || (url.StartsWith("images/"))) return false;

            // If we get here, we're OK handling it
            return true;
        }
    }
}