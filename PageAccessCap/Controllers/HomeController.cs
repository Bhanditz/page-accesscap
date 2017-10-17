using PageAccessCap.Filters;
using System.Web.Mvc;

namespace PageAccessCap.Controllers
{
    public class HomeController : BaseController
    {
        [PageAccess(threshold: 3)]
        public ActionResult Index()
        {
            return View();
        }
    }
}