using System.Web.Mvc;

namespace University_Enrollment_System.Controllers
{
    public class MainController : Controller
    {
        // GET: /Login
        public ActionResult MainHome()
        {
            // View is located at Views/Main/Home.cshtml
            return View("~/Views/Main/Home.cshtml");
        }

        public ActionResult Student_Profile()
        {
            // View is located at Views/Main/StudentProfile.cshtml
            return View("~/Views/Main/StudentProfile.cshtml");
        }

        public ActionResult Student_Enrollment()
        {
            // View is located at Views/Main/StudentEnroll.cshtml
            return View("~/Views/Main/StudentEnroll.cshtml");
        }

        public ActionResult Student_Grade()
        {
            // View is located at Views/Main/ViewGrades.cshtml
            return View("~/Views/Main/ViewGrades.cshtml");
        }

        public ActionResult Student_Schedule()
        {
            // View is located at Views/Main/ClassSchedule.cshtml
            return View("~/Views/Main/ClassSchedule.cshtml");
        }
    }
}