using System.Web.Mvc;

namespace University_Enrollment_System.Controllers
{
    public class ProgramHeadController : Controller
    {
        // GET
        public ActionResult Dashboard()
        {
            return View("~/Views/Program-Head/Dashboard.cshtml");
        }

        public ActionResult Students()
        {
            return View("~/Views/Program-Head/ViewStudentList.cshtml");
        }

        public ActionResult Approval()
        {
            return View("~/Views/Program-Head/EnrollmentApproval.cshtml");
        }

        public ActionResult Schedule()
        {
            return RedirectToAction("Index", "Schedule");
        }

        public ActionResult StudentManagement()
        {
            return View("~/Views/Program-Head/StudentManagement.cshtml");
        }

        public ActionResult ClassManagement()
        {
            return View("~/Views/Program-Head/ClassManagement.cshtml");
        }
    }
}