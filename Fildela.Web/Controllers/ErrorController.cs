using Fildela.Resources;
using System.Configuration;
using System.Web.Mvc;

namespace Fildela.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        [AllowAnonymous]
        public ActionResult Default()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Error;

            return View();
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Error;

            return View();
        }

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Error;

            return View();
        }

        [AllowAnonymous]
        public ActionResult HttpError()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Error;

            return View();
        }
    }
}
