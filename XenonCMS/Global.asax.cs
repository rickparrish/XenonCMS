using System;
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

                // TODO Maybe handle CMS pages here too?  ie look for slug and execute controller action if slug is valid

                if (SiteHelper.FileExists(Context.Request.RequestContext.HttpContext, Request.FilePath))
                {
                    var FileRD = new RouteData();
                    FileRD.DataTokens["area"] = "";
                    FileRD.Values["controller"] = "Cms";
                    FileRD.Values["action"] = "File";
                    FileRD.Values["filename"] = Request.FilePath;

                    try
                    {
                        IController FileC = new CmsController();
                        FileC.Execute(new RequestContext(new HttpContextWrapper(Context), FileRD));
                        Context.Response.StatusCode = 200;
                        return;
                    }
                    catch (Exception)
                    {
                        // 404 on file handler, fall through to friendly 404 below
                        // TODO Log the exception
                        // TODO Shouldn't fall through as 404 since we confirmed it existed above, what error should it be, 500?
                    }
                }

                // If we get here the file handler failed, so show a friendly 404
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
