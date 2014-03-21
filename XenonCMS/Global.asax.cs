using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XenonCMS.Controllers;

namespace XenonCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // From http://stackoverflow.com/a/9026907
        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 400)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "BadRequest";

                IController c = new ErrorsController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
            else if (Context.Response.StatusCode == 403)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "NotAuthorized";

                IController c = new ErrorsController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
            else if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "NotFound";

                IController c = new ErrorsController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
            else if (Context.Response.StatusCode == 500)
            {
                if (!Debugger.IsAttached)
                {
                    Response.Clear();

                    var rd = new RouteData();
                    rd.DataTokens["area"] = "";
                    rd.Values["controller"] = "Errors";
                    rd.Values["action"] = "InternalServerError";

                    IController c = new ErrorsController();
                    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                }
            }
        }
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
