using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XenonCMS.Controllers;
using XenonCMS.Classes;

namespace XenonCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // From http://stackoverflow.com/a/9026907
        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 400)
            {
                SendErrorResponse("BadRequest");
            }
            else if (Context.Response.StatusCode == 403)
            {
                SendErrorResponse("NotAuthorized");
            }
            else if (Context.Response.StatusCode == 404)
            {
                SendErrorResponse("NotFound");
            }
            else if (Context.Response.StatusCode == 500)
            {
                var ex = Server.GetLastError();

                try
                {
                    // TODO Log/email the exception and message
                }
                catch (Exception ex2)
                {
                    // Ignore email failure
                }

                try
                {
                    SendErrorResponse("InternalServerError");
                }
                catch (Exception ex2)
                {
                    // TODO Log/email the exception and message
                    Response.Redirect("~/500.html");
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

        private void SendErrorResponse(string action)
        {
            Response.Clear();

            var rd = new RouteData();
            rd.DataTokens["area"] = "";
            rd.Values["controller"] = "Errors";
            rd.Values["action"] = action;

            ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
        }
    }
}
