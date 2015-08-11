using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XenonCMS.Controllers;
using XenonCMS.Helpers;

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

                if (SiteHelper.FileExists(Context.Request.RequestContext.HttpContext, Request.FilePath))
                {
                    var rd = new RouteData();
                    rd.DataTokens["area"] = "";
                    rd.Values["controller"] = "Cms";
                    rd.Values["action"] = "File";
                    rd.Values["filename"] = Request.FilePath;

                    IController c = new CmsController();
                    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));

                    Context.Response.StatusCode = 200;
                }
                else
                {
                    var rd = new RouteData();
                    rd.DataTokens["area"] = "";
                    rd.Values["controller"] = "Errors";
                    rd.Values["action"] = "NotFound";

                    IController c = new ErrorsController();
                    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                }
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

        protected void Application_Error()
        {
            var LastError = Server.GetLastError();
            if (LastError is HttpException)
            {
                if (((HttpException)LastError).GetHttpCode() == 404)
                {
                    // Send us a 404 message, but then return so the standard 404 error message will display
                    //Logging.LogException(Nothing, "<h2>404</h2><p>" & Request.Url.AbsolutePath & "</p>")
                    return;
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
