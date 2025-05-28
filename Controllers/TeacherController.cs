using System.Web.Mvc;

namespace University_Enrollment_System.Controllers
{
    public class TeacherController : Controller
    {
        public ActionResult Dashboard()
        {
            return View("~/Views/Teacher/Dashboard.cshtml");
        }

        public ActionResult Classes()
        {
            return View("~/Views/Teacher/Classes.cshtml");
        }
    }
}