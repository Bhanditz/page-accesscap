using PageAccessCap.Filters;
using System.Web.Mvc;

namespace PageAccessCap.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [PageAccess(threshold: 3)]
        public ActionResult Login()
        {
            var model = string.Empty;

            return Json(model);
        }
    }
}