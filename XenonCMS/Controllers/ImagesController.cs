using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using XenonCMS.Helpers;

namespace XenonCMS.Controllers
{
    public sealed class ImagesController : Controller
    {
        public FilePathResult Image(string filename)
        {
            string ImagesDiretory = HostingEnvironment.MapPath("~/Images/" + Globals.GetRequestDomain(ControllerContext.HttpContext));
            string RequestedFile = Path.Combine(ImagesDiretory, filename);

            if (RequestedFile.ToLower().StartsWith(ImagesDiretory.ToLower()) && System.IO.File.Exists(RequestedFile)) 
            {
                return File(RequestedFile, "image/" + Path.GetExtension(RequestedFile).ToLower().Trim('.'));
            }
            else
            {
                throw new HttpException(404, "Not found");
            }
        }
    }
}