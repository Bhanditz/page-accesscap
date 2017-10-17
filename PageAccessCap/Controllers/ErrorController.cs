using PageAccessCap.Models;
using System.Web.Mvc;

namespace PageAccessCap.Controllers
{
    public partial class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult Forbidden()
        {
            var model = new ErrorViewModel(ErrorMessage);

            return View(model);
        }

        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }
    }

    public partial class ErrorController
    {
        public const string ErrorModelKey = "__ErrorModel";

        private string ErrorMessage
        {
            get
            {
                return TempData[ErrorModelKey] as string;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!TempData.ContainsKey(ErrorModelKey))
            {
                TempData.Add(ErrorModelKey, null);
                filterContext.Result = RedirectToAction("NotFound");
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}