using System;
using System.Web.Hosting;
using System.Web.Mvc;
using XenonCMS.Helpers;

namespace XenonCMS.Controllers
{
    public sealed class ImagesController : Controller
    {
        public FilePathResult Image(Guid id)
        {
            string Filename = HostingEnvironment.MapPath("~/Images/" + Globals.GetRequestDomain(ControllerContext.HttpContext) + "/" + id + ".png");
            return File(Filename, "application/octet-stream");
        }
    }
}