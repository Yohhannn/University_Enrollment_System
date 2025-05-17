using System.Web.Mvc;

namespace University_Enrollment_System.Controllers
{
    public class MainController : Controller
    {
        // GET
        public ActionResult Index()
        {
            ViewBag.Name = "Main";
            return View();
        }
    }
}