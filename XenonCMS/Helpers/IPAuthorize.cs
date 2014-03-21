using System.Web.Mvc;

namespace XenonCMS.Helpers
{
    class IPAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Globals.IsUserFromAdminIP(filterContext.RequestContext.HttpContext))
            {
                // IPs don't match, so don't let the user in.
                filterContext.RequestContext.HttpContext.Response.StatusCode = 403;
            }
        }
    }
}