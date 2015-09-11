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
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "BadRequest";

                ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
            else if (Context.Response.StatusCode == 403)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "NotAuthorized";

                ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
            else if (Context.Response.StatusCode == 404)
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
                        throw new Exception("asdf");
                        ((IController)new CmsController()).Execute(new RequestContext(new HttpContextWrapper(Context), SlugRD));
                        return;
                    }
                    catch (Exception ex)
                    {
                        // TODO Log the exception
                        Context.Response.StatusCode = 500;
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
                        // TODO Log the exception
                        Context.Response.StatusCode = 500;
                    }
                }
                else if (Globals.IsNewSite(new HttpContextWrapper(Context)))
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
                        // TODO Log the exception
                        Context.Response.StatusCode = 500;
                    }
                }


                // If we get here the file handler failed, so show a friendly 404
                var rd = new RouteData();
                rd.DataTokens["area"] = "";
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = (Response.StatusCode == 404) ? "NotFound" : "InternalServerError"; // Slug and File handlers may have thrown a 500 above

                ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
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

                    ((IController)new ErrorsController()).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
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
