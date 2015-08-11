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
            // TODO Does this work?  If so, add for above functions too
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
    }
}