using System.Web.Mvc;

namespace XenonCMS.Controllers
{
    public sealed class ErrorsController : Controller
    {
        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}