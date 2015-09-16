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
                try
                {
                    Response.Clear();
                    if (SiteHelper.SlugExists(Context.Request.RequestContext.HttpContext, Request.FilePath))
                    {
                        var SlugRD = new RouteData();
                        SlugRD.DataTokens["area"] = "";
                        SlugRD.Values["controller"] = "Cms";
                        SlugRD.Values["action"] = "Slug";
                        SlugRD.Values["slug"] = Request.FilePath;

                        try
                        {
                            Context.Response.StatusCode = 200;
                            ((IController)new CmsController()).Execute(new RequestContext(new HttpContextWrapper(Context), SlugRD));
                            return;
                        }
                        catch (Exception ex)
                        {
                            SendError500Response(ex, "Application_EndRequest, 404, SlugExists");
                            return;
                        }
                    }
                    else if (SiteHelper.FileExists(Context.Request.RequestContext.HttpContext, Request.FilePath))
                    {
                        var FileRD = new RouteData();
                        FileRD.DataTokens["area"] = "";
                        FileRD.Values["controller"] = "Cms";
                        FileRD.Values["action"] = "File";
                        FileRD.Values["filename"] = Request.FilePath;

                        try
                        {
                            Context.Response.StatusCode = 200;
                            ((IController)new CmsController()).Execute(new RequestContext(new HttpContextWrapper(Context), FileRD));
                            return;
                        }
                        catch (Exception ex)
                        {
                            SendError500Response(ex, "Application_EndRequest, 404, FileExists");
                            return;
                        }
                    }
                    else if (!SiteHelper.SiteExists(new HttpContextWrapper(Context)))
                    {
                        Response.Clear();

                        var InstallRD = new RouteData();
                        InstallRD.DataTokens["area"] = "";
                        InstallRD.Values["controller"] = "Cms";
                        InstallRD.Values["action"] = "Install";

                        try
                        {
                            Context.Response.StatusCode = 200;
                            ((IController)new CmsController()).Execute(new RequestContext(new HttpContextWrapper(Context), InstallRD));
                            return;
                        }
                        catch (Exception ex)
                        {
                            SendError500Response(ex, "Application_EndRequest, 404, !SiteExists");
                            return;
                        }
                    }

                    // If we get here the file handler failed, so show a friendly 404
                    var rd = new RouteData();
                    rd.DataTokens["area"] = "";
                    rd.Values["controller"] = "Errors";
                    rd.Values["action"] = (Response.StatusCode == 404) ? "NotFound" : "InternalServerError"; // Slug and File handlers may have thrown a 500 above

                    ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                }
                catch (Exception ex)
                {
                    SendError500Response(ex, "Application_EndRequest, 404");
                    return;
                }
            }
            else if (Context.Response.StatusCode == 500)
            {
                SendError500Response(null, "Application_EndRequest, 500");
                return;
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

        private void SendError500Response(Exception ex, string Message)
        {
            if (ex == null) ex = Server.GetLastError();

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
}
