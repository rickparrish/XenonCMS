using System.Web.Mvc;

namespace XenonCMS.Helpers
{
    class GlobalAdminIPAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Globals.IsUserFromGlobalAdminIP(filterContext.RequestContext.HttpContext))
            {
                // IPs don't match, so don't let the user in.
                filterContext.Result = new HttpStatusCodeResult(403);
            }
        }
    }
}